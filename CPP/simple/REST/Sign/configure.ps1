# usage  -WSDL $WSDL -libwsock32 $libwsock32

[CmdletBinding()]
param (
    
    [parameter(HelpMessage="path inside cpp-httplib lib folder (default: .\lib\cpp-httplib)")]
    [string]$PATH_cpp_httplib,

    [parameter(HelpMessage="path inside json lib folder (default: .\lib\json)")]
    [string]$PATH_json
)

Write-Output "Configuring Project"

#cpp-httplib
if(!$PATH_cpp_httplib) {
    $PATH_cpp_httplib = Read-Host -Prompt "Please enter path to cpp-httplib lib (default: .\lib\cpp-httplib)"
    if(!($PATH_cpp_httplib)) { $PATH_cpp_httplib = ".\lib\cpp-httplib"}
}
if(!(Test-Path -Path $PATH_cpp_httplib)) {
    Write-Output "cpp-httplib lib not found at $PATH_cpp_httplib"
    Exit
}

#cut '\' if at the end
if($PATH_cpp_httplib.Substring($PATH_cpp_httplib.get_Length()-1) -eq "\") {
    $PATH_cpp_httplib = $PATH_cpp_httplib.Substring(0,$PATH_cpp_httplib.get_Length()-1)
}

#json
if(!$PATH_json) {
    $PATH_json = Read-Host -Prompt "Please enter path to cpp-httplib lib (default: .\lib\json)"
    if(!($PATH_json)) { $PATH_json = ".\lib\json"}
}
if(!(Test-Path -Path $PATH_json)) {
    Write-Output "json lib not found at $PATH_json"
    Exit
}

#cut '\' if at the end
if($PATH_json.Substring($PATH_json.get_Length()-1) -eq "\") {
    $PATH_json = $PATH_json.Substring(0,$PATH_json.get_Length()-1)
}

#write makefile
$IN = (Get-Content -Path .\Makefile.in -Raw)

$VARS = "PATH_cpp_httplib = `"$PATH_cpp_httplib`"`n"
$VARS += "PATH_json = `"$PATH_json`""

$OUT = "$VARS`n`n$IN"
Out-File -FilePath .\Makefile -InputObject $OUT -Encoding utf8

$CURRENT = Get-Location
cd $PATH_cpp_httplib
python3 split.py
cd $CURRENT

Write-Output "Configuration sucessfull"