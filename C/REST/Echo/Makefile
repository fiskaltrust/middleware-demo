ifeq ($(OS),Windows_NT) #Windows

CFLAGS = -IC:/dev_gcc/curltut/curl-7.67.0/include
LDFLAGS= -L"C:/dev_gcc/curltut/curl-7.67.0/build/Win64/VC15/DLL Release - DLL OpenSSL - DLL LibSSH2" -llibcurl

main.exe: src\main.c
	gcc.exe src\main.c -o build\main.exe $(CFLAGS) $(LDFLAGS) -ggdb -Wall

#Linux
else

main: src/main.c
	mkdir build -p
	gcc src/main.c -o build/echo -lcurl

endif




