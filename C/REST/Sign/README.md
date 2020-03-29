# Intro

This example sends a signing request to the fiskaltrust.Service via REST.

A zero receipt is sent to a specified url and CashBox and the response is printed.

> _**Note:** If the http statuscode 204 is returned the CashBox may not yet be activated and you'll need to send a start receipt first. For instructions on how to do this please consult our videos about [sending requests to the fiskaltrust.Service](https://www.youtube.com/playlist?list=PL9QFfhi6nFj94kZBTxxL3kyar2Q7yTejU)_

This example can be used as a starting point to implement the [fiskaltrust.Interface](https://github.com/fiskaltrust/interface-doc)

> _**Note:** The german cash transaction is being implemented at the moment in the fiskaltrust.Service. Since the implementation is similar, you can use the austrian service for testing._

# Requirements

## Toolchain

To compile the example the `C` compiler [`gcc`](https://gcc.gnu.org/install/) and the buildtool [`make`](https://www.gnu.org/software/make/) is needed. To build and install the joson-c library [`cmake`](https://cmake.org/) is also required

> _**Note:** On windows we recommend installing these with a package manager like [scoop](https://scoop.sh/) or [chocolatey](https://chocolatey.org/)_

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

After the library is compiled, the file `json-c.dll` has to be copied to `C:\Windows\System32` or to `.\build\`.

### Linux

To complile the library follow [those](https://github.com/json-c/json-c#build-instructions) instructions.

> _**Note:** You may also have to look into the [Install prerequisites](https://github.com/json-c/json-c#install-prerequisites-)._

# Building

## Linux

  1. Run the `make` command.

## Windows

  1. Run the powershell script `configure.ps1` and follow the instructions.
  2. Run the `make` command.

> _**Example:**_
> ```
> .\configure.ps1
> make
> ```
