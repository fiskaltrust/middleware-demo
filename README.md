# fiskaltrust.interface EN
Examples how to use fiskaltrust.interface.

fiskaltrust offers a legal compliant cash register security mechanism.

## News 14.11.2016
Per now we published our release candidate interface, version 1.0.nnn. This interface is included in the Nuget Package fiskaltrust.interface and fiskaltrust.interface.utlities version 1.0.16298.1022-rc. The WSDL description file here at Github in the tools directory changed as well.

## Documentation
The detailed documentation for Austria is available from fiskaltrust-portal [https://portal.fiskaltrust.at] and for France is available from fiskaltrust-portal [https://portal.fiskaltrust.fr] when after you activate the role possystem-creator (Registrierkassenhersteller).

To speed up development we also deliver a nuget-package [https://nuget.org] with the packageId fiskaltrust.interface.

## Connecting to fiskaltrust.securitymechanism
As a base technology in communication WCF is used. For local internal communication between queues, signature creation units and custom modules the net.pipe protocoll is the best choice. For multi platform communication the basic http protocol may be the best choice.
### SOAP
SOAP comes with the http protocol from WCF communication. To get the WSDL file you can use these debug-build and goto the http-address configured, here [http://localhost:1200/0b09d163-82a1-4349-83ed-7081398df504] is used. Another option is to use the file from the folder tools/wsdl.
### REST
REST is available in both, in XML and JSON. There are helpers which can be loaded to keep the base service lightweight.
### native TCP-IP and serial RS232/485/422
Native stream based communication with a defined protocoll format is provided by helpers.
### User specific
With the helper topology it is possible to solve every scenario.

## Hosting on Linux and MacOS
For usage on Linux and MacOS we use mono to run the fiskaltrust service on it.

For production use it is possible to run it as a daemon.

For test and development, the command-line parameter -test can be used.
(You can find details in the developer documentation.)

Prerequisites beside mono-complete 3.x / 4.x are also SQlite and PCSClite if you want to use an usb-based signature creation unit.

Typical commands to run:
```sudo apt-get update```    
```sudo apt-get install mono-complete```    
```sudo apt-get install sqlite```    
```sudo apt-get install pcsclite```    
```cd fiskaltrust-mono```    
```sudo mono fiskaltrust.mono.exe -caschboxid=0d1269dc-e2ae-42e3-9c57-b686d7832683 -useoffline=true -test```

## Hosting on Microsoft Windows
The launcher (fiskaltrust.exe) is constructed to act as an Microsoft Windows service in a production environment. For this also automated installation is supported by command-line parameter. ((You can find details in the developer documentation.)

For test and develop the command-line parameter -test can be used to run the service.

The only prerequisites for running the service on a Windows-based machine is .net4.

Typical commands to run:    
(open command-line with administration permission)    
```cd fiskaltrust-net40```    
```fiskaltrust -cashboxid=0d1269dc-e2ae-42e3-9c57-b686d7832683 -test```

## Cloud based
The same interface and service definition is served as an cloud service. You can use SOAP and REST interface designed for local service to seamless switch over to the cloud service.

## Test related informations
The launcher uses the file configuration.json from its execution directory to make up its basic configuration. In production use this is done in the fiskaltrust-portal and the launcher tries to read it from the upload-server, related to the CashboxID and an generated AccessToken. For offline use this configuration is stored in the execution directory. Once the configuration is readed from the execution directory or from upload-server, it is stored localy in the service-folder. The default service-foler is for Microsoft Windows %ProgramData%\fiskaltrust or on Linux /usr/shared/fiskaltrust.

In this folder also the database file and the executeables are stored, to completly reset the service delete this directory.

## Card testing
In the tools\cardtest folder is a tool to test card and readers online and offline. See the README.md included there. The usage of so called cards (a SignatureCreation Unit) is only obligatory when using the service for Austria.

## Common error
Due to security reasons, the fiskaltrust.securitymechanism does not return anything (null) if the provided CashboxID is wrong. 

## Feedback and bugs
The fiskaltrust service is under permanent development, so feel free to discuss here your wishes and our bugs with the github-issues feature.

## fiskaltrust consulting gmbh
Lemböckgasse 49/1B/6.OG, 1230 Wien  
[info@fiskaltrust.at]  
[www.fiskaltrust.at](https://www.fiskaltrust.at)

---

# fiskaltrust.Interface
[see english readme](#fiskaltrust-interface-EN)

Beispiel, wie man das fiskaltrust.Interface nutzt.

fiskaltrust bietet eine gesetzeskonforme Sicherheitseinrichtung für Registrierkassen.

##Neuerungen 14.11.2016
Ab sofort gibt es ein neues Interface (unseren Release Candidate), Version 1.0.nnn. Dieses Interface ist im Nuget Package fiskaltrust.interface und fiskaltrust.interface.utlities Version 1.0.16298.1022-rc enthalten. Ebenfalls geändert hat sich dadurch die Wsdl Datei hier in den Tools.

## Dokumentation
Die detaillierte Dokumentation ist auf dem fiskaltrust.Portal [https://portal.fiskaltrust.at] nach der Registrierung und Aktivierung der Rolle als Registrierkassenhersteller (in der Übersicht) verfügbar.

Um Ihren Entwicklung zu erleichtern, stellen wir auch ein nuget-package [https://nuget.org] mit der packageId fiskaltrust.interface zur Verfügung.

## Verbindung mit der fiskaltrust.securitymechanism (Sicherheitseinrichtung)
Als Basis-Technologie zur Kommunikation wird WCF verwendet. Zur lokalen, internen Kommunikation zwischen queues, signature creation units (Signaturerstellungseinheiten) und benutzerspezifischen Modulen (Sonstigen Modulen) wird am besten das net.pipe protocoll verwendet. Zur Kommunikation zwischen verschiedenen Plattformen wird am besten basic http verwendet.
### SOAP
SOAP wird mit dem http-Protokoll der wcf-Kommunikation ausgeliefert. Um die WSDL-Datei zu erhalten, kann man diesen Debug-Build verwenden und auf die konfigurierte http-Adresse gehen. Hierbei wird [http://localhost:1200/0b09d163-82a1-4349-83ed-7081398df504] verwendet. Eine weitere Option besteht darin, die Datei aus dem Ordner tools/wsdl zu verwenden.

### REST
REST steht sowohl in XML als auch in JSON zur Verfügung. Es stehen benutzerspezifische Module zur Verfügung, die geladen werden können, um die Basis-Services möglichest schlank zu halten.
### native TCP-IP und Serielle RS232/485/422
Nativen Stream-basierte Kommunikation mit einem definierten Protokoll-Format wird als benutzerspezifische Module zur Verfügung gestellt.
### Benutzerspezifisch
Mit der Topologie der benutzerspezifischen Module ist es möglich, jedes technische Szenario zu lösen.

## Hosting auf Linux und Mac OS
Um fiskaltrust auf Linux und Mac OS laufen zu lassen, wird mono verwenden.
Für den produktiven Einsatz ist es möglich, ihn mit daemon zu betreiben.

Für Test und Entwicklung kann der command-line Parameter -test verwendet werden. Details sind in der Entwickler-Dokumentation zu finden.

Neben Mono-complete 3.x / 4.x ist auch SQLite und pcsclite Voraussetzung, wenn eine USB-basierte Signaturerstellungseinheit verwendet werden soll.

Typisch ausführbare Befehle:    
```sudo apt-get update```    
```sudo apt-get install mono-complete```    
```sudo apt-get install sqlite```    
```sudo apt-get install pcsclite```    
```cd fiskaltrust-mono```    
```sudo mono fiskaltrust.mono.exe -caschboxid=0d1269dc-e2ae-42e3-9c57-b686d7832683 -useoffline=true -test```

## Hosting unter Windows
Der launcher (fiskaltrust.exe) ist als Windows-Dienst für die Produktionsumgebung entwickelt. Es wird auch die Möglichkeit einer automatisierte Installation durch Befehlszeilenparameter unterstützt. Details sind in der Entwickler-Dokumentation zu finden.

Für die Entwicklung und Prüfung kann der Befehlszeilenparameter -test verwendet werden um den Dienst auszuführen.

Hierfür ist nur .net4 Voraussetzung.

Typisch ausführbare Befehle:
(command-line mit Administrationsrecht starten)    
```cd fiskaltrust-net40```    
```fiskaltrust -cashboxid=0d1269dc-e2ae-42e3-9c57-b686d7832683 -test```

## Cloud-basiert
Die gleiche Schnittstellen- und service-Definitionen werden als Cloud-Service unterstützt. Das als lokales Service entwickelte SOAP- und REST-Interface kann nahtlos in einen Cloud-Service gewechselt werden.

## Informationen zu Tests
Der Launcher verwendet die Datei configuration.json von seinem Ausführungsverzeichnis um die Basiskonfiguration aufzubauen. Im Produktionsbetrieb wird diese Konfiguration im fiskaltrust-Portal vorgenommen und der launcher versucht, diese vom Upload-Server auszulesen um die cashboxid und den accesstoken zu beziehen. Zur Offline-Nutzung wird diese Konfiguration im Ausführungsverzeichnis gespeichert. Sobald die Konfiguration aus dem Ausführungsverzeichnis oder von Upload-Server ausgelesen wird, wird es im lokalen Service-Ordner gespeichert. Der Standard-Service-Ordner ist unter Windows %Programdata%\fiskaltrust oder in Linux /usr/shared/fiskaltrust. In diesem Ordner werden auch die Datenbankdatei und die ausführbaren Dateien gespeichert.

Um den Dienst vollständig zurückzusetzen, kann das komplette Verzeichnis gelöscht werden.

## Kartentests
Im Verzeichnis Tools\cardtest ist ein Werkzeug zum Testen von Karten und Readern, online und offline. Dort befindet sich auch eine README.md mit einer Beschreibung.

## Häufiger Fehler
Aus Sicherheitsgründen reagiert die fiskaltrust.Sicherheitseinrichtung nur mit einer gültigen Antwort, wenn eine korrekte CashboxID in den Daten übergeben wurde. Wenn eine unbekannte übergeben wird, so kommt keine Antwort zurück.

## Feedback und Bugs
fiskaltrust wird ständig weiterentwickelt. Nutzen Sie bitte die Möglichkeit, durch Github-Fragen Ihre Wünsche und unsere Fehler zu diskutieren.

## Fiscaltrust consulting gmbh
Lemböckgasse 49/1B/6.OG, 1230 Wien  
[info@fiskaltrust.at]  
[www.fiskaltrust.at](https://www.fiskaltrust.at)
