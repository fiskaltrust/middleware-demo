# usage  -WSDL $WSDL -libwsock32 $libwsock32

[CmdletBinding()]
param (
    
    [parameter(HelpMessage="path inside cpp-httplib lib folder (default: .\lib\cpp-httplib)")]
    [string]$PATH_cpp_httplib,

    [parameter(HelpMessage="path inside openssl lib folder (default: .\lib\openssl)")]
    [string]$PATH_openssl
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

#openssl
if(!$PATH_openssl) {
    $PATH_openssl = Read-Host -Prompt "Please enter path to cpp-httplib lib (default: .\lib\openssl)"
    if(!($PATH_openssl)) { $PATH_openssl = ".\lib\openssl"}
}
if(!(Test-Path -Path $PATH_openssl)) {
    Write-Output "openssl lib not found at $PATH_openssl"
    Exit
}
#cut '\' if at the end
if($PATH_openssl.Substring($PATH_openssl.get_Length()-1) -eq "\") {
    $PATH_openssl = $PATH_openssl.Substring(0,$PATH_openssl.get_Length()-1)
}

#write makefile
$IN = (Get-Content -Path .\Makefile.in -Raw)

$VARS = "PATH_cpp_httplib = `"$PATH_cpp_httplib`"`n"
$VARS += "PATH_openssl = `"$PATH_openssl`""

$OUT = "$VARS`n`n$IN"
Out-File -FilePath .\Makefile -InputObject $OUT -Encoding utf8

$CURRENT = Get-Location
cd $PATH_cpp_httplib
python3 split.py
cd $CURRENT

Write-Output "Configuration sucessfull"