ifeq ($(OS),Windows_NT) #Windows

echo.exe: src\main.cpp
	mkdir build || echo "dir exisit"
	g++ -o build\echo.exe src\main.cpp lib\cpp-httplib\out\httplib.cc -D CPPHTTPLIB_OPENSSL_SUPPORT -L.\ -l:"$(wildcard ./build/libssl*)" -l:"$(wildcard ./build/libcrypto*)" -lws2_32 -std=c++14 -ggdb 

else #Linux

main: src/main.cpp
	mkdir build -p
	gcc src/main.cpp -o build/echo

endif




