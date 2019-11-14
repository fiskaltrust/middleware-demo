# usage  -WSDL $WSDL -libwsock32 $libwsock32

[CmdletBinding()]
param (
    
    [parameter(HelpMessage="path inside gSOAP lib folder(example: .\lib\gsoap-2.8")]
    [string]$PATH_gSOAP,

    [parameter(HelpMessage="example: path\to\file\name.wsdl")]
    [String]$WSDL_PATH,
    
    [parameter()]
    [String]$libwsock32_PATH
)

Write-Output "Configuring Project"

#gSOAP
if(!$PATH_gSOAP) {
    $PATH_gSOAP = Read-Host -Prompt "Please enter path to gsaop lib(example: .\lib\gsoap-2.8)"
}
if(!(Test-Path $PATH_gSOAP)) {
    Write-Output "gSAOP lib not found at $PATH_gSOAP"
    Exit
}

#WSDL
if(!$WSDL_PATH) {
    $WSDL_PATH = Read-Host -Prompt 'location to WSDL file'
}

if(!(Test-Path $WSDL_PATH)) {
    Write-Output "wsdl file not found at $WSDL_PATH"
    Exit
}

#libwsock32.a
if(!$libwsock32_PATH) {
    Write-Output "If you know were your libwsock32.a libray is please enter the path, if not enter nothing and we will look for it"
    $libwsock32_PATH = Read-Host -Prompt 'location to libwsock32.a'
}

If(!$libwsock32_PATH) {
    #find file
    Write-Output "searcing for file, this can take some minutes"
    
    if([array]$TMP_directory = Get-ChildItem -Path C:\Windows\ -include libwsock32.a -ErrorAction SilentlyContinue -recurse | Select-Object -First 1 Directory) {
        foreach($Directory in $TMP_directory) {$libwsock32_PATH = $Directory.Directory.FullName}
    }

    elseif([array]$TMP_directory = Get-ChildItem -Path C:\Users\ -include libwsock32.a -ErrorAction SilentlyContinue -recurse | Select-Object -First 1 Directory) {
        foreach($Directory in $TMP_directory) {$libwsock32_PATH = $Directory.Directory.FullName}
    }

    elseif([array]$TMP_directory = Get-ChildItem -Path C:\ -include libwsock32.a -ErrorAction SilentlyContinue -recurse | Select-Object -First 1 Directory) {
        foreach($Directory in $TMP_directory) {$libwsock32_PATH = $Directory.Directory.FullName}
    }

    else {
        Write-Output "No libwsock32.a found"
        Exit
    }
    
}

$tmp = "$libwsock32_PATH\libwsock32.a"
if (!(Test-Path $tmp)) {
    Write-Output "libwsock32.a not found at $libwsock32_PATH\"
    Exit
}

#write makefile

#load makefile.in
$IN = (Get-Content -Path .\Makefile.in -Raw)

$VARS = "PATH_to_WSDL = $WSDL_PATH"
$VARS = "$VARS`nPATH_to_gSOAP = $PATH_gSOAP"
$VARS = "$VARS`nPATH_to_libwsock32 = $libwsock32_PATH"

$OUT = "$VARS`n`n$IN"
Out-File -FilePath .\Makefile -InputObject $OUT -Encoding utf8

Write-Output "Configuration sucessfull"