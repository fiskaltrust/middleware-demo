ifeq ($(OS),Windows_NT) #Windows

main.exe: main.c
	gcc.exe main.c -o build\main.exe -IC:/dev_gcc/curltut/curl-7.67.0/include -L"C:/dev_gcc/curltut/curl-7.67.0/build/Win64/VC15/DLL Release - DLL OpenSSL - DLL LibSSH2" -llibcurl -ggdb -Wall

#Linux
else

main: main.c
	gcc main.c -o build\main -lcurl

endif




