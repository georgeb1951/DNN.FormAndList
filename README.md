# BCCForms

BCCForms is a DNN 10 module for building data-entry forms and lists: define your own fields
(text, numbers, dates, files, images, users, lookups, calculated expressions), validation and
per-row permissions, then show the data as a sortable grid or through your own XSL template.

It started as a fork of [DNN Form and List](https://github.com/DNNCommunity/DNN.FormAndList)
6.5.9 (MIT licence, DNN Community) and is a **separate module**: its own name, install folder,
assembly, database tables and permission code. It installs side by side with Form and List and
does not upgrade or read Form and List data.

## Requirements

- DNN Platform **10.3.3** or later
- .NET Framework 4.8
- SQL Server 2017 or later (same as DNN 10)

## Identity

| Item | Value |
| --- | --- |
| Package / module name | `BCCForms` |
| Friendly name | BCC Forms |
| Install folder | `DesktopModules/BCCForms` |
| Assembly | `BCC.Modules.BCCForms.dll` |
| Root namespace | `BCC.Modules.BCCForms` |
| Tables | `BCCForms_Fields`, `BCCForms_Rows`, `BCCForms_Data`, `BCCForms_FieldSettings` |
| Stored procedures | `BCCForms_*` (22) |
| Permission code | `BCCFORMS` (keys `ROWEDIT`, `ROWDELETE`, `ROWADD`, `COLUMNVISIBLE`, `PRIVATECOLUMNS`, `SHOWLIST`) |
| Custom data-type config | `Portals/_Default/BCCForms/datatypes.config` |

## Building

1. Open `BCCForms.sln` in Visual Studio 2022.
2. Restore NuGet packages (DNN 10.3.3 packages, compile-only).
3. The project is meant to live in a DNN dev site at `DesktopModules\BCCForms`; the build output
   path is the site's `bin` (`..\..\bin`). DNN and Microsoft assemblies are referenced with
   `Private=False`, so a build never overwrites the site's own copies.
4. A **Release** build also creates `packages\BCCForms_01.00.00_Install.zip` (and a `_Source.zip`)
   via `BuildScripts\ModulePackage.targets`.

## Notes for templates

The XML that XSL templates receive keeps Form and List's contract (namespace
`DotNetNuke/UserDefinedTable`, root element `udt:UserDefinedTable`, `[UDT:...]` tokens), so
existing Form and List XSL templates work unchanged.

## Licence

MIT. See [LICENSE](LICENSE); the original DNN Community copyright notice is retained as the
licence requires.
