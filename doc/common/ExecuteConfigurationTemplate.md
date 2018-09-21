# ExecuteConfigurationTemplate 1.0.0.0

Copyright ©  2018

## Command Line Parameters:
  
| Parameter 			  	| Mandatory | Type		| Description 																						|
| --------------------------|:---------:|:---------:|---------------------------------------------------------------------------------------------------|
| --helipadurl, --u			| Required	| String	| Service url for fiskaltrust helipad. Default: "https://helipad-sandbox.fiskaltrust.at/"			|
| --accountid, --i			| Required	| GUID		| API Account Id for the accessing the configuration. (GUID formatted)								|
| --accesstoken, --a		| Required	| String	| API Accesstoken for the used account.																|
| --template, --t			| Required	| String	| Template for the creation of the cashbox. (JSON serialization)									|
| --help   					| Optional	| -			| Display the help screen.																			|
| --version   				| Optional	| -			| Display the version information.																	|