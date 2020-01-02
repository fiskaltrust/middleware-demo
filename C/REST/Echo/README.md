# Intro

This example sends a echo request to the fiskaltrust.Service via REST.

An echo request with a custom message is sent to a specified url and CashBox and the response is printed.

This example can be used as a starting point to implement the [fiskaltrust.Interface](https://github.com/fiskaltrust/interface-doc).

# Requirements

## Toolchain

To compile the example the `C` compiler [`gcc`](https://gcc.gnu.org/install/) and the buildtool [`make`](https://www.gnu.org/software/make/) is needed.

> _**Note:** On windows we recomend installing these with a package manager like [scoop](https://scoop.sh/) or [chocolatey](https://chocolatey.org/)_

## curl

To build the example, the [curl library](https://curl.haxx.se/libcurl/) needs to be available. You can download this library from [here](https://curl.haxx.se/download.html).

> _**Note:** We tested the example with version 7.x.x other versions may not work._

### Windows

Building the curl library for windows is a complicated process. [This](https://albertino80.github.io/building.html) guide is a good starting point.

After the library is compiled all the following dll files have to be added to system32 or next to the executable.<br>`libcrypto-1_1.dll, zlibwapi.dll, nghttp2.dll, libssl-1_1.dll, libssh2.dll, libcurl.dll`

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

# Building

## Linux

  1. Run the `make` command.

## Windows

  1. Adapt the file paths in the makefile to your locations for libcurl.

  2. Run the `make` command.
