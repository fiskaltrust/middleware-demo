ifeq ($(OS),Windows_NT) #Windows

main.exe: src\main.c
	gcc.exe src\main.c -o build\main.exe -IC:/dev_gcc/curltut/curl-7.67.0/include -L"C:/dev_gcc/curltut/curl-7.67.0/build/Win64/VC15/DLL Release - DLL OpenSSL - DLL LibSSH2" -llibcurl -ggdb -Wall

#Linux
else

main: src/main.c
	mkdir build -p
	gcc src/main.c -o build/Journal -lcurl -I/usr/local/include -L/usr/local/lib -l:libjson-c.a
endif