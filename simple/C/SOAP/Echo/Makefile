PATH_to_WSDL = C:\Users\JonathanKoenig\Downloads\fiskaltrust_fiskaltrust.interface.1.0.16298.1022.wsdl
PATH_to_gSOAP = C:\Users\JonathanKoenig\OneDrive - fiskaltrust consulting gmbh\Dokumente\Programs\C\SOAP_C_Echo\lib\gsoap-2.8
PATH_to_libwsock32 = C:\Users\JonathanKoenig\scoop\apps\gcc\8.1.0\x86_64-w64-mingw32\lib

ifeq ($(OS),Windows_NT)
echo.exe: src\main.c soapC.c
	gcc -o echo.exe src\main.c "$(PATH_to_gSOAP)\gsoap\stdsoap2.c" "$(PATH_to_gSOAP)\gsoap\bin\win32\soapC.c" "$(PATH_to_gSOAP)\gsoap\bin\win32\soapClient.c" -I"$(PATH_to_gSOAP)\gsoap\bin\win32" -I"$(PATH_to_gSOAP)\gsoap" -L"$(PATH_to_libwsock32)" -l:libwsock32.a -Wall

soapC.c:
	"$(PATH_to_gSOAP)\gsoap\bin\win32\wsdl2h.exe" -c -s -o "$(PATH_to_gSOAP)\gsoap\bin\win32\ftservice.h" "$(PATH_to_WSDL)"
	"$(PATH_to_gSOAP)\gsoap\bin\win32\soapcpp2.exe" -CL -c -1 "$(PATH_to_gSOAP)\gsoap\bin\win32\ftservice.h" -I"$(PATH_to_gSOAP)\gsoap\import" -r  -d"$(PATH_to_gSOAP)\gsoap\bin\win32"

else
echo: soapC.c


soapC.c:

endif
