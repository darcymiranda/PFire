# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src\PFire.Common\PFire.Common.csproj
4. Upgrade src\PFire.Infrastructure\PFire.Infrastructure.csproj
5. Upgrade src\PFire.Core\PFire.Core.csproj
6. Upgrade src\PFire.Console\PFire.Console.csproj
7. Upgrade tests\PFire.Tests\PFire.Tests.csproj
8. Run unit tests to validate upgrade in the projects listed below:
  tests\PFire.Tests\PFire.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                 | Current Version | New Version | Description                                   |
|:---------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.EntityFrameworkCore                |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.Design         |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.Sqlite         |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting.Abstractions    |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting.Systemd         |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting.WindowsServices |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Logging.Abstractions    |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Options.ConfigurationExtensions |   9.0.1         |  10.0.0     | Recommended for .NET 10.0                     |

### Project upgrade details

#### src\PFire.Common\PFire.Common.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting.Abstractions should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging.Abstractions should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)

#### src\PFire.Infrastructure\PFire.Infrastructure.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Sqlite should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)

#### src\PFire.Core\PFire.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### src\PFire.Console\PFire.Console.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting.WindowsServices should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting.Systemd should be updated from `9.0.1` to `10.0.0` (*recommended for .NET 10.0*)

#### tests\PFire.Tests\PFire.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

