ifeq ($(OS),Windows_NT) #Windows

echo.exe: src\main.c libcurl
#	gcc.exe src\main.c -o build\echo.exe -I./include -L.\lib\static-release-x64 -llibcurl_a -Wall
	gcc src\main.c -o build\echo.exe libcurl -Wall

libcurl:
	

#Linux
else

main: main.c
	gcc src/main.c -o build/main -lcurl

endif




