ifeq ($(OS),Windows_NT) #Windows

all:
	echo "In Windows, please first run the configure.ps1"

#Linux
else

main: src/main.c
	mkdir build -p
	gcc src/main.c -o build/journal -lcurl

endif
