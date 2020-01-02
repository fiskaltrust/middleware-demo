ifeq ($(OS),Windows_NT) #Windows

JSON_C_DIR="C:\Program Files\json-c"
CFLAGS  = -I$(JSON_C_DIR)\include
LDFLAGS = -L$(JSON_C_DIR)\lib -ljson-c

CFLAGS += -IC:/dev_gcc/curltut/curl-7.67.0/include
LDFLAGS+= -L"C:/dev_gcc/curltut/curl-7.67.0/build/Win64/VC15/DLL Release - DLL OpenSSL - DLL LibSSH2" -llibcurl

sign.exe: src\main.c
	if not exist .\build mkdir build
	gcc.exe src\main.c -o build\sign.exe $(CFLAGS) $(LDFLAGS) -ggdb -Wall

#Linux
else

main: src/main.c
	mkdir build -p
	gcc src/main.c -o build/Sign -lcurl -I/usr/local/include -L/usr/local/lib -l:libjson-c.a
endif