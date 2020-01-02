# Intro

This example sends a signing request to the fiskaltrust.Service via REST.

A zero receipt is sent to a specified url and CashBox and the response is printed.

> _**Note:** If the http statuscode 204 is returned the CashBox may not yet be activated and you'll need to send a start receipt first. For instructions on how to do this please consult our videos about [sending requests to the fiskaltrust.Service](https://www.youtube.com/playlist?list=PL9QFfhi6nFj94kZBTxxL3kyar2Q7yTejU)_

This example can be used as a starting point to implement the [fiskaltrust.Interface](https://github.com/fiskaltrust/interface-doc)

> _**Note:** The german cash transaction is being implemented at the moment in the fiskaltrust.Service. Since the implementation is similar, you can use the austrian service for testing._

# Requirements

## Toolchain

To compile the example the `C` compiler [`gcc`](https://gcc.gnu.org/install/) and the buildtool [`make`](https://www.gnu.org/software/make/) is needed.

> _**Note:** On windows we recommend installing these with a package manager like [scoop](https://scoop.sh/) or [chocolatey](https://chocolatey.org/)_

## curl

To build the example, the [curl library](https://curl.haxx.se/libcurl/) needs to be available. You can download this library from [here](https://curl.haxx.se/download.html).

> _**Note:** We tested the example with version 7.x.x other versions may not work._

### Windows

Building the curl library for windows is a complicated process. [This](https://albertino80.github.io/building.html) guide is a good starting point.

After the library is compiled the following dll files have to be copied to `C:\Windows\System32` or to `.\build\`.<br>`libcrypto-1_1.dll, zlibwapi.dll, nghttp2.dll, libssl-1_1.dll, libssh2.dll, libcurl.dll`

> _**Note:** If the program is executed from window it throws error about the dll it is missing._

Also the curl certificate has to be obtained.<br> Downloaded the Windows curl from [curl](https://curl.haxx.se/windows/) and copy the `curl-ca-bundle.crt` to the project folder.

> _**Note:** We do not recommend working with C and REST on windows since building curl takes a lot of effort._

### Linux

Install the library via your distributions package manager.

| Distribution  | Command                    |
|---------------|----------------------------|
| Ubuntu/Debian | `apt-get install libcurl4 libcurl4-openssl-dev` |
| Fedora        | `dnf install libcurl`                           |
| OpenSUSE      | `zypper install libcurl4 libcurl4-openssl-dev`  |

## json-c

[This](https://github.com/json-c/json-c) JSON parser is used in this project.

### Windows

Build the library using the following commands:

```
mkdir build
cd build
cmake ../
cmake --build . --target install
```

> _**Note:** To run these commands the [VS build tools](https://aka.ms/buildtools) need to be installed._

After the library is compiled the dll the `json-c.dll` has to be copied to `C:\Windows\System32` or to `.\build\`.

### Linux

To complile the library follow [those](https://github.com/json-c/json-c#build-instructions) instructions.

> _**Note:** You may also have to look into the [Install prerequisites](https://github.com/json-c/json-c#install-prerequisites-)._

# Building

## Linux

  1. Run the `make` command.

## Windows

  1. Adapt the file paths in the makefile to your locations for libcurl and json-c.

  2. Run the `make` command.
