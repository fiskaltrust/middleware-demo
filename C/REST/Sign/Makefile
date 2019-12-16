ifeq ($(OS),Windows_NT) #Windows

main.exe: main.c
	gcc.exe src\main.c -o build\main.exe -IC:/dev_gcc/curltut/curl-7.67.0/include -L"C:/dev_gcc/curltut/curl-7.67.0/build/Win64/VC15/DLL Release - DLL OpenSSL - DLL LibSSH2" -llibcurl -ggdb -Wall

#Linux
else

main: src/main.c lib/cJSON/cJSON.a
	mkdir build -p
	gcc src/main.c -o build/Sign -lcurl lib/cJSON/cJSON.a -Ilib/

lib/cJSON/cJSON.a: lib/cJSON/cJSON.c
	gcc -c lib/cJSON/cJSON.c -o lib/cJSON/cJSON.a
endif




