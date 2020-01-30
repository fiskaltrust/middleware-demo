REM "c:\program files\microsoft sdks\windows\v7.1\bin\wsutil.exe" /wsdl:e131service.wsdl /noservice /nopolicy /string:WS_STRING  /ignoreUnhandledAttributes
REM "c:\program files\microsoft sdks\windows\v7.1\bin\wsutil.exe" /wsdl:e131service.wsdl /string:WCHAR*
REM "c:\program files\microsoft sdks\windows\v7.1\bin\wsutil.exe" /wsdl:e131service.wsdl /string:WS_STRING  /ignoreUnhandledAttributes
..\..\..\tools\wsutil\x86\wsutil.exe /wsdl:fiskaltrust.ifPOS.xml /string:WS_STRING  /ignoreUnhandledAttributes

