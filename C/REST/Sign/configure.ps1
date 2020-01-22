# usage  -WSDL $WSDL -libwsock32 $libwsock32

[CmdletBinding()]
param (
    
    [parameter(HelpMessage="path inside curl lib folder (default: .\lib\curl)")]
    [string]$PATH_libcurl,

    [parameter(HelpMessage="path inside json-c lib folder (default: C:\Program Files\json-c)")]
    [string]$PATH_jsonc
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

#libcurl
if(!$PATH_jsonc) {
    $PATH_jsonc = Read-Host -Prompt "Please enter path to json-c lib (default: C:\Program Files\json-c)"
    if(!($PATH_jsonc)) { $PATH_jsonc = "C:\Program Files\json-c"}
}
if(!(Test-Path $PATH_jsonc)) {
    Write-Output "curl lib not found at: $PATH_jsonc"
    Exit
}

#cut '\' if at the end
if($PATH_libcurl.Substring($PATH_libcurl.get_Length()-1) -eq "\") {
    $PATH_libcurl = $PATH_libcurl.Substring(0,$PATH_libcurl.get_Length()-1)
}

if($PATH_jsonc.Substring($PATH_jsonc.get_Length()-1) -eq "\") {
    $PATH_jsonc = $PATH_jsonc.Substring(0,$PATH_jsonc.get_Length()-1)
}

#write makefile
$IN = (Get-Content -Path .\Makefile.in -Raw)

$VARS = "CFLAGS = -I`"$PATH_libcurl"
$VARS += "\include`"`n"
$VARS += "CFLAGS += -I`"$PATH_jsonc"
$VARS += "\include`"`n"
$VARS += "LDFLAGS = -L`"$PATH_jsonc"
$VARS += "\lib`" -ljson-c"

$OUT = "$VARS`n`n$IN"
Out-File -FilePath .\Makefile -InputObject $OUT -Encoding utf8

Write-Output "Configuration sucessfull"
