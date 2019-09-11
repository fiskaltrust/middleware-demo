// cppConsoleApplicationSOAP.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <time.h>

#include <WebServices.h>

#include "wsdl\fiskaltrust.ifPOS.xml.h"


int main()
{

	HRESULT hr = NOERROR;

	//Fehlerspeicher vorbereiten
	WS_ERROR* error = NULL;
	hr = WsCreateError(NULL, 0, &error);

	//Nachrichtenspeicher vorbereiten
	//Unbedingt die maximale Nachrichtengröße (maxSize) auf 65536 setzen, da dies der Standardwert ist beim .NET 4
	WS_HEAP* heap = NULL;
	hr = WsCreateHeap(/*maxSize*/ 65536, 512, NULL, 0, &heap, error);

	//Endpunkt vorbereiten
	WS_ENDPOINT_ADDRESS address = {};
	WS_STRING url = WS_STRING_VALUE(L"http://localhost:1200/ec6e38eb-80e4-4452-a704-6ec9ee375663");
	address.url = url;

	//Proxy erstellen und öffnen
	WS_SERVICE_PROXY* proxy = NULL;
	WS_HTTP_BINDING_TEMPLATE templ = {};
	hr = BasicHttpBinding_IPOS_CreateServiceProxy(&templ, NULL, 0, &proxy, error);

	hr = WsOpenServiceProxy(proxy, &address, NULL, error);
	if (FAILED(hr))
	{
		ULONG errorCount;
		hr = WsGetErrorProperty(error, WS_ERROR_PROPERTY_STRING_COUNT, &errorCount, sizeof(errorCount));
		if (!FAILED(hr))
		{
			for (ULONG i = 0; i < errorCount; i++)
			{
				WS_STRING errorMessage;
				hr = WsGetErrorString(error, i, &errorMessage);
				if (!FAILED(hr))
				{
					wprintf(L"%.*s\n", errorMessage.length, errorMessage.chars);
				}
			}
		}
	}

	//echo-Funktion, zum Verbindungstes und Vorwärmen des Dienstes falls dieser neu gestartet wurde
	WS_STRING echoInMessage = WS_STRING_VALUE(L"Hello World!");
	WS_STRING echoOutMessage;
	hr = BasicHttpBinding_IPOS_Echo(proxy, echoInMessage.chars, &echoOutMessage.chars, heap, NULL, 0, NULL, error);
	if (!FAILED(hr))
	{
		wprintf(L"%.*s\n", echoOutMessage.length, echoOutMessage.chars);
	}
	else
	{
		ULONG errorCount;
		hr = WsGetErrorProperty(error, WS_ERROR_PROPERTY_STRING_COUNT, &errorCount, sizeof(errorCount));
		if (!FAILED(hr))
		{
			for (ULONG i = 0; i < errorCount; i++)
			{
				WS_STRING errorMessage;
				hr = WsGetErrorString(error, i, &errorMessage);
				if (!FAILED(hr))
				{
					wprintf(L"%.*s\n", errorMessage.length, errorMessage.chars);
				}
			}
		}
	}





	//Speicher aufräumen
	if (proxy)
	{
		WsCloseServiceProxy(proxy, NULL, NULL);
		WsFreeServiceProxy(proxy);
	}

	if (heap)
	{
		WsFreeHeap(heap);
	}

	if (error)
	{
		WsFreeError(error);
	}

    return 0;
}

