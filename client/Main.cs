using CitizenFX.FiveM.Client;
using CitizenFX.FiveM.Shared.Script;

using vMenu.Enhanced.ClientAPI;

namespace ThemePicker.Client;

public sealed class Main : IScript
{
    // Lives in this resource's own key value store on the player's computer, never on the server.
    private const string SavedThemeKey = "theme";

    private readonly Dictionary<string, PluginButton> _buttons = new(StringComparer.OrdinalIgnoreCase);

    private readonly List<string> _built = new();

    private VMenuPlugin _plugin = null!;

    private PluginButton? _reset;

    private bool _restored;

    public async void Initialize()
    {
        _plugin = VMenuPlugin.Create(Text.Key("themes.name"));

        _plugin.DescriptionKey = "themes.description";

        AddTranslations(_plugin);

        _plugin.RootMenu.Subtitle = Text.Key("themes.subtitle");

        // The rows are built from what vMenu sends, which lands here right after registering and
        // again on every theme change, so nothing is added before the list exists.
        _plugin.Themes.Changed += Sync;

        // vMenu forgets an override when it restarts, so the saved theme is put back on the next one.
        _plugin.Disconnected += () => _restored = false;

        var result = await _plugin.ConnectAsync();

        API.Log.Info($"[ThemePicker] Registered with vMenu: {result.Accepted}.");
    }

    private void Sync()
    {
        var themes = _plugin.Themes.Available;

        if (!MatchesBuilt(themes))
        {
            Rebuild(themes);
        }

        Restore(themes);

        foreach (var theme in themes)
        {
            if (_buttons.TryGetValue(theme.Id, out var button))
            {
                button.Label = theme.IsCurrent ? Text.Key("themes.current") : Text.Empty;
            }
        }

        if (_reset is { } reset)
        {
            reset.Enabled = _plugin.Themes.IsOverridden;
        }
    }

    private void Rebuild(IReadOnlyList<PluginTheme> themes)
    {
        // One update rather than one per row, so vMenu repaints the menu once.
        using (_plugin.BeginBatch())
        {
            _plugin.RootMenu.Clear();

            _buttons.Clear();
            _built.Clear();

            foreach (var theme in themes)
            {
                var id = theme.Id;

                var button = _plugin.RootMenu.AddButton(Text.Literal(theme.Name), id: "theme." + id);

                button.Description = Text.Key("themes.pick", ("theme", Text.Literal(theme.Name)));
                button.Selected += () => Pick(id);

                _buttons[id] = button;
                _built.Add(id);
            }

            _reset = _plugin.RootMenu.AddButton(Text.Key("themes.reset"), id: "theme.reset");
            _reset.Description = Text.Key("themes.reset.desc");
            _reset.Selected += Forget;
        }
    }

    private void Pick(string id)
    {
        _restored = true;

        Native.SetResourceKvp(SavedThemeKey, id);

        _plugin.Themes.Set(id);
    }

    private void Forget()
    {
        _restored = true;

        Native.DeleteResourceKvp(SavedThemeKey);

        _plugin.Themes.Reset();
    }

    // Once per vMenu, and only while the saved theme is one vMenu currently offers: the resource that
    // provides it may still be starting, and then it lands on a later list.
    private void Restore(IReadOnlyList<PluginTheme> themes)
    {
        if (_restored || Native.GetResourceKvpString(SavedThemeKey) is not { Length: > 0 } saved)
        {
            return;
        }

        foreach (var theme in themes)
        {
            if (!string.Equals(theme.Id, saved, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            _restored = true;

            if (!theme.IsCurrent)
            {
                _plugin.Themes.Set(theme.Id);
            }

            return;
        }
    }

    private bool MatchesBuilt(IReadOnlyList<PluginTheme> themes)
    {
        if (_built.Count != themes.Count)
        {
            return false;
        }

        for (var index = 0; index < themes.Count; index++)
        {
            if (!string.Equals(_built[index], themes[index].Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    // Every language needs the same keys, and English is the one vMenu falls back to when the
    // player's language has no table here.
    private static void AddTranslations(VMenuPlugin plugin)
    {
        plugin.Translations.Add("en", new Dictionary<string, string>
        {
            ["themes.name"] = "Theme Picker",
            ["themes.description"] = "Changes how vMenu looks, for you only.",
            ["themes.subtitle"] = "Menu Themes",

            ["themes.pick"] = "Draws every vMenu menu in the {theme} theme. Only you see it, and it is remembered for the next time you play.",
            ["themes.current"] = "Current",

            ["themes.reset"] = "Use the server's theme",
            ["themes.reset.desc"] = "Puts the menus back in the theme the server picked and forgets your choice. This row wakes up once you have chosen a theme yourself.",
        });

        plugin.Translations.Add("nl", new Dictionary<string, string>
        {
            ["themes.name"] = "Themakiezer",
            ["themes.description"] = "Verandert hoe vMenu eruitziet, alleen voor jou.",
            ["themes.subtitle"] = "Menuthema's",

            ["themes.pick"] = "Tekent elk vMenu menu in het thema {theme}. Alleen jij ziet het, en het wordt onthouden voor de volgende keer.",
            ["themes.current"] = "Huidige",

            ["themes.reset"] = "Gebruik het thema van de server",
            ["themes.reset.desc"] = "Zet de menu's terug in het thema dat de server heeft gekozen en vergeet je keuze. Deze rij wordt wakker zodra je zelf een thema hebt gekozen.",
        });
    }
}
