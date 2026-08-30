# vMenu Theme Picker

A plugin for [vMenu Enhanced](https://github.com/TomGrobbe/vMenu) that lets every player pick the theme they see, instead of the one the server picked. Their choice is saved on their own computer and comes back next time. No permissions, no settings.

## Installing

Take the zip from [the latest release](https://github.com/TomGrobbe/vMenu.ThemePicker/releases/latest), put the `vMenu.ThemePicker` folder in your server's `resources`, and add `ensure vMenu.ThemePicker` to your server config. Start order does not matter.

In game it sits under vMenu's Plugins menu, one row per theme plus one to go back to the server's theme.

## Building it yourself

.NET 10 SDK, then `dotnet build -c Release`. The resource lands in `build/vMenu.ThemePicker/`.

It builds against the `vMenu.Enhanced.ClientAPI` package pinned in `Directory.Packages.props`, so keep that in step with the vMenu your server runs. Against a vMenu older than `0.0.1-alpha.92` it starts fine but finds no themes, since that vMenu cannot send them.

Want more themes than the four vMenu ships with? See the [Custom Themes plugin](https://github.com/TomGrobbe/vMenu.CustomThemesPlugin).

## License

GPL-3.0-or-later, the same license vMenu Enhanced and its plugin API use.
