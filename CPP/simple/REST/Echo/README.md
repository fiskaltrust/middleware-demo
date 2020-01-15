# Intro

This example sends a echo request to the fiskaltrust.Service via REST.

An echo request with a custom message is sent to a specified url and CashBox and the response is printed.

This example can be used as a starting point to implement the [fiskaltrust.Interface](https://github.com/fiskaltrust/interface-doc).

# Requirements

## Toolchain

To compile the example the `MinGW` compiler [`g++`](http://www.mingw.org/) and the buildtool [`make`](https://www.gnu.org/software/make/) are needed.

> _**Note:** On windows we recomend installing these with a package manager like [scoop](https://scoop.sh/) or [chocolatey](https://chocolatey.org/)_

## cpp-httplib

Please download the from [here](https://github.com/yhirose/cpp-httplib).

> _**Note:** The configure script will as the location of the library (default: .\lib\cpp-httplib)_

### Windows

cpp-httplib needs the openssl and zlib library to be able to send secure https requests.

please downlaod the prebuild openssl and zlib from [here](https://curl.haxx.se/windows/)

please copy `libssl*.dll`, `libcrypto*.dll` and `zlib*.a` to the `.\build\` folder

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
