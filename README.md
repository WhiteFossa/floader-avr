# floader-avr

## About
This is AVR bootloader, written in assembler with crossplatform GUI tool to upload/download firmware to/from devices.

![Client app screenshot](https://raw.githubusercontent.com/WhiteFossa/floader-avr/master/client.png)

## Features
- bootloader is written in AVR assembler (can be compiled with either AVR Studio or AVRA);
- while written in assembler, it is portable - all hardware-dependent code placed into separate HAL, only HAL need to be adapted to add support for new MCU;
- bootloader supports device identification, sending to client vendor ID (VID), model ID (MID) and serial number, so user can check if he/she uploads correct firmware;
- client software is crossplatform (written in .NET Core C# + Avalonia) and tested both on GNU/Linux (Fedora) and Windows 10;
- client software contains database, allowing to show textual description of devices, based on their VID/MID and check for firmware correctness (for example it will refuse to flash if firmware size differs from MCU FLASH size);
- client software automatically creates backups, which can be re-uploaded into device if something went wrong;
- NO support for lock bits - forced opensource :3
- AGPLv3+ license;
- well-documented, enterprise-style code;
- both Russian and English are supported.

## License
AGPLv3 or newer version at your option.

## Used 3rd party code:
- https://www.nuget.org/packages/System.IO.Ports/ (MIT)
- dotnet add package Avalonia.ReactiveUI (MIT)
- dotnet add package System.Data.SQLite (Public domain)
- dotnet add package Dapper (Apache 2.0)
- https://www.nuget.org/packages/MessageBox.Avalonia/ (Apache 2.0)
