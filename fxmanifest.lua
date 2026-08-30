-- vMenu Enhanced theme picker plugin.
--
-- This file is copied into build/vMenu.ThemePicker/ on every build, next to the client folder the
-- project writes into. Copy that whole folder into your server's resources and
-- `ensure vMenu.ThemePicker`.

fx_version 'cerulean'
games { 'gta5' }

name 'vMenu Theme Picker'
description 'Lets a player pick the vMenu theme they see, for the rest of their session.'
author 'Tom Grobbe'

-- Everything the client half needs, because a client assembly and its dependencies are
-- downloaded rather than read off the server's disk. Add a line here for every DLL that
-- shows up in build/vMenu.ThemePicker/client/ after adding a package reference.
files {
    'client/CitizenFX.Base.dll',
    'client/CitizenFX.FiveM.Shared.dll',
    'client/CitizenFX.FiveM.Client.dll',

    'client/MessagePack.dll',
    'client/MessagePack.Annotations.dll',

    'client/Microsoft.NET.StringTools.dll',

    'client/vMenu.Enhanced.PluginContracts.dll',
    'client/vMenu.Enhanced.ClientAPI.dll',
}

client_script 'client/ThemePicker.Client.dll'
