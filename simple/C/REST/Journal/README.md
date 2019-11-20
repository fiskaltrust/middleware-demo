# Intro

This is a simple REST example which send an Journal request to a fiskaltrust.service and prints the response.

# Install
## Windows

### Library
We do not recomend working with C in Windows because the comilation of the libcurl Library is very long and complicated.

If you get the libcurl Library running the programm works

[This](https://albertino80.github.io/building.html) link is a good starting point but you may have to adapt it a bit.

### Building
To build you first have to run the configure.ps1 and then build with `make`

## Linux

### Library

just install the libcurl on your PC

for Ubuntu: `sudo apt install libcurl4*`

### building

just run `make`
