# usage  -WSDL $WSDL -libwsock32 $libwsock32

[CmdletBinding()]
param (
    
    [parameter(HelpMessage="path inside curl lib folder (default: .\lib\curl)")]
    [string]$PATH_libcurl
)

Write-Output "Configuring Project"

#libcurl
if(!$PATH_libcurl) {
    $PATH_libcurl = Read-Host -Prompt "Please enter path to curl lib (default: .\lib\curl)"
    if(!($PATH_libcurl)) { $PATH_libcurl = ".\lib\curl"}
}
if(!(Test-Path $PATH_libcurl)) {
    Write-Output "curl lib not found at: $PATH_libcurl"
    Exit
}

#cut '\' if at the end
if($PATH_libcurl.Substring($PATH_libcurl.get_Length()-1) -eq "\") {
    $PATH_libcurl = $PATH_libcurl.Substring(0,$PATH_libcurl.get_Length()-1)
}

#write makefile
$IN = (Get-Content -Path .\Makefile.in -Raw)

$VARS = "CFLAGS = -I$PATH_libcurl"
$VARS += "\include"

$OUT = "$VARS`n`n$IN"
Remove-Item Makefile
Out-File -FilePath .\Makefile -InputObject $OUT -Encoding utf8

Write-Output "Configuration sucessfull"
