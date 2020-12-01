set ABS_PATH=%CD%
sc.exe create "PFireServer" binpath="%CD%\PFire.WindowsService.exe" displayname="PFire Server"
sc.exe description "PFireServer" "Emulated XFire Server"