# csConsoleApplicationJournalREST 1.0.0.0

Copyright ©  2018

## Command Line Parameters:
  
| Parameter 			  	| Mandatory | Type		| Description 																						|
| --------------------------|:---------:|:---------:|---------------------------------------------------------------------------------------------------|
| --fiskaltrust-service-url	| Required  | String	| Url of the running fiskaltrust.service. Default: "https://signaturcloud-sandbox.fiskaltrust.at/"	|
| --cashboxid				| Required	| GUID		| API Cashbox Id for the accessing the configuration. (GUID formatted)								|
| --accesstoken				| Required	| String	| API Accesstoken for the used cashbox.																|
| --json					| Required  | Boolean	| Is the serialization in JSON format (true) or XML format (false).									|
| --help					| Optional  | -			| Display the help screen.																			|
| --version					| Optional  | -			| Display the version information.																	|