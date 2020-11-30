# PFire
Emulated XFire server (Client 1.127)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Discord](https://img.shields.io/discord/619547253702393856.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/jWPWZu8DPy)

![.NET Core](https://github.com/darcymiranda/PFire/workflows/.NET%20Core/badge.svg)

## Build
Using .NET 5

`dotnet build`

### Run & Connect:
1. Add cs.xfire.com to your hosts file with your IP (for Windows it's located here `C:\Windows\System32\drivers\etc\hosts`)
  e.g. 
  ```
  127.0.0.1 cs.xfire.com
  ```
  > _Note: This is a work around to redirect the XFire client to point to a your local server (127.0.0.1) instead of the real server_
2. Within the PFire.Console directory, use command `dotnet run`
3. Login with your desired XFire client (I've only tested with v1.127), an account will be created automatically

> _Note: You can find a copy of the v1.127 XFire client [here](https://www.dropbox.com/s/fjj5u0uksg6t46f/Xfire.rar?dl=0). If you don't trust the url, just google it._

# Working features:
* Friend search
* Friend requests
* Statuses
* 1 to 1 chat messaging

### Screenshots
![Screenshot of XFire connecting to PFire](readme-screenshot.png)
