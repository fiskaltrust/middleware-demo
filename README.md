# fiskaltrust.Middleware demos (C, C++, VB6 & Rust)
Demo applications that demonstrate how to call the fiskaltrust.Middleware from C, C++, VB6, Rust, and from within Excel. This repository contains examples for **WCF** and **REST** based communication, using both JSON and XML.


## Getting Started

### Prerequisites
In order to use these demo applications, different prerequisites need to be fulfilled due to the different programming languages. Therefore, please follow the steps described in the respective readme files in the demo subdirectories to prepare your environment.

For more complete examples in other programming languages, please refer to the last section of this document.

### Repository Structure
This repository contains a SOAP and a REST/HTTP sample program for each endpoint of the fiskaltrust.Middleware. 

The repository is structured the following way: 
```
/<programming-language>/<SOAP|REST>/<Echo|Sign|Journal>/
```

> _**Example:** [`/C/REST/Sign/`](/C/REST/Sign)_

### Advanced
In addition to these simple examples, this repository also contains more advanced approaches that were partially created to demonstrate the support for specific customer demands. These can be found in:
```
/<programming-language>/advanced/
```


## Documentation
The full documentation for the interface can be found on https://docs.fiskaltrust.cloud. It is actively maintained and developed in our [interface-doc repository](https://github.com/fiskaltrust/interface-doc). 

More information is also available after logging into the portal with a user that has the _PosCreator_ role assigned.

### Communication
The fiskaltrust.Middleware supports different communication protocols, effectively giving our customers the possibility to use it on all platforms. Hence, different protocols are recommended for different platforms. For non-windows environments, we recommend the usage of gRPC. Please have a look into our other demo repositories for alternatives, e.g. HTTP/REST or SOAP.

#### User specific protocols
With the helper topology, it is possible to solve every scenario. Please contact our support if you required assistance for a special case scenario.

## Contributions
We welcome all kinds of contributions and feedback, e.g. via Issues or Pull Requests. 

## Related resources
Our latest samples are available for the following programming languages and tools:
<p align="center">
  <a href="https://github.com/fiskaltrust/middleware-demo-dotnet"><img src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7a/C_Sharp_logo.svg/100px-C_Sharp_logo.svg.png" alt="csharp"></a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="https://github.com/fiskaltrust/middleware-demo-java"><img src="https://upload.wikimedia.org/wikiversity/de/thumb/b/b8/Java_cup.svg/100px-Java_cup.svg.png" alt="java"></a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="https://github.com/fiskaltrust/middleware-demo-node"><img src="https://upload.wikimedia.org/wikipedia/commons/thumb/d/d9/Node.js_logo.svg/100px-Node.js_logo.svg.png" alt="node"></a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="https://github.com/fiskaltrust/middleware-demo-android"><img src="https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Android_robot.svg/100px-Android_robot.svg.png" alt="android"></a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="https://github.com/fiskaltrust/middleware-demo-postman"><img src="https://avatars3.githubusercontent.com/u/10251060?s=100&v=4" alt="node"></a>
</p>
