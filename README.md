# floader-avr
Almost ten years ago I've wrote opensource (GPLv3) bootloader for AVR MCUs (in assmebler) and client software for it (in C++/Qt). I've abandoned that project, but found, that is still being used at least by one person.

So, I'm going to revive it, refactor assembler part, and rewrite client in .NET Core.

Notes for myself (later will format it):
Under *nix xclip have to be installed.

Used 3rd party code:
- https://github.com/CopyText/TextCopy (MIT)
- https://www.nuget.org/packages/System.IO.Ports/ (MIT)
- dotnet add package Avalonia.ReactiveUI (MIT)
- dotnet add package System.Data.SQLite (Public domain)
- dotnet add package Dapper (Apache 2.0)
