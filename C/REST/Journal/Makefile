CFLAGS = -I.\lib\curl-7.69.1-win64-mingw\include

ifeq ($(OS),Windows_NT) #Windows

LDFLAGS = -L./
LDFLAGS += -l:$(wildcard build/libcurl*)
LDFLAGS += -l:$(wildcard build/libbrot*)
LDFLAGS += -l:$(wildcard build/libcrypto*)
LDFLAGS += -l:$(wildcard build/libnghttp*)
LDFLAGS += -l:$(wildcard build/libssh*)
LDFLAGS += -l:$(wildcard build/libssl*)
LDFLAGS += -l:$(wildcard build/libz*)

journal.exe: src\main.c
	mkdir build || echo "dir exisit"
	gcc.exe src\main.c -o build\journal.exe $(CFLAGS) $(LDFLAGS) -ggdb -Wall

#Linux
else

journal: src/main.c
	mkdir build -p
	gcc src/main.c -o build/journal -lcurl

endif
