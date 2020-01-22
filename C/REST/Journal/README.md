# Intro

This example sends a journal request to the fiskaltrust.Service via REST.

A journal request is sent to a specified url and CashBox and the response is printed.

This example can be used as a starting point to implement the [fiskaltrust.Interface](https://github.com/fiskaltrust/interface-doc).

# Requirements

## Toolchain

To compile the example the `C` compiler [`gcc`](https://gcc.gnu.org/install/) and the buildtool [`make`](https://www.gnu.org/software/make/) is needed.

> _**Note:** On windows we recomend installing these with a package manager like [scoop](https://scoop.sh/) or [chocolatey](https://chocolatey.org/)_

## curl

To build the example, the [curl library](https://curl.haxx.se/libcurl/) needs to be available.

> _**Note:** We tested the example with version 7.x.x, other versions may not work._

### Windows

The curl library can be downloaded as pre compiled libraries. Please downlaod the prebuild libraries (curl, OpenSSL, brotli, libssh2, nghttp2, zlib) from [here](https://curl.haxx.se/windows/)

Please copy `libbrotlicommon-static.a`, `libcurl*.dll`, `libnghttp*.a`, `libssh*.dll`,`libssl*.dll`, `libcrypto*.dll` and `libz*.a` to the `.\build\` folder

> _**Note:** This list of libraries will be reminded by executing the program from a window, in the error message._

curl certificate is included in the curl zip file, next to the libcurl*.dll, copy  `curl-ca-bundle.crt` to this project folder.

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

  1. Run the `.\configure.ps1`

  2. Run the `make` command.
