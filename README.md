# vMenu Theme Picker

A small plugin for [vMenu Enhanced](https://github.com/TomGrobbe/vMenu) that lets a player choose the look of their own vMenu menus, from inside vMenu itself.

vMenu ships a few themes: the default soft dark glass look, a solid dark one, a bright cartoon one, and the plain GTA V style. Normally the server owner picks one for everybody with a convar. This plugin adds a Theme Picker row to vMenu's Plugins menu with a button for every theme vMenu knows about, and pressing one switches the menus over straight away.

**The choice belongs to the player and to that session only.** Nothing is written to disk, so reconnecting to the server, restarting the game, or restarting the resource puts the server's own theme back. There is one more row, Use the server's theme, that does the same thing without waiting for a reconnect. It stays greyed out until you have actually picked a theme yourself, because before that there is nothing to undo.

There are no permissions and no settings. Anyone who can open vMenu can change how their own menus look, which is the point: it changes nothing but what that one player sees.

## Building it

You need the .NET 10 SDK. From this folder:

```
dotnet build -c Release
```

The finished resource lands in `build/vMenu.ThemePicker/`. Copy that whole folder into your server's `resources`, then add `ensure vMenu.ThemePicker` to your server config. It has to start alongside vMenu, in either order: the plugin waits for vMenu and registers itself again if vMenu restarts.

## The package it builds against

The plugin talks to vMenu through the `vMenu.Enhanced.ClientAPI` NuGet package, pinned in `Directory.Packages.props` to `0.0.1-alpha.92`, which is the first vMenu Enhanced release whose plugin API can list and set themes.

Keep that pin in step with the vMenu your server actually runs. If you move vMenu to a newer release, raise the version here too, and if your server still runs something older than `0.0.1-alpha.92`, this plugin will register fine but find no themes to offer, because that vMenu does not know how to send them.

## How it works

The whole plugin is one file, `client/Main.cs`, and the interesting part is short:

```csharp
_plugin.Themes.Changed += Sync;
```

vMenu sends the theme list to a plugin right after it registers, and again every time the theme changes, whoever changed it. So the rows are built from that event rather than up front, and the Current label moves on its own when something else changes the theme, including the server owner editing the convar while you play.

Picking a theme is `_plugin.Themes.Set(id)`, and going back to the server's choice is `_plugin.Themes.Reset()`.

## License

GPL-3.0-or-later, the same license vMenu Enhanced and its plugin API use.
