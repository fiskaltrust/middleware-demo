#include <BasicHttpBinding_USCOREIPOS.nsmap>
#include <soapH.h>
#include <inttypes.h> //int64_t, uint64_t
#include <stdio.h>    /* printf, sprintf */
#include <stdlib.h>   /* exit, atoi, malloc, free */
#include <string.h>

#define STRING_LENGTH 256

#ifdef _WIN32
    #define type_Echo_request _ns1__Echo
    #define type_Echo_response _ns1__EchoResponse
    #define soap_call_echo(soap, endpoint, action, Echo_request, Echo_response) soap_call___ns1__Echo(soap, endpoint, action, Echo_request, Echo_response)
#else //Linux
    #define type_Echo_request _tempuri__Echo
    #define type_Echo_response _tempuri__EchoResponse
    #define soap_call_echo(soap, endpoint, action, Echo_request, Echo_response) soap_call___tempuri__Echo(soap, endpoint, action, Echo_request, Echo_response)
#endif

char *ltrim(char *str, const char *seps) {
    size_t totrim;
    if (seps == NULL) {
        seps = "\t\n\v\f\r ";
    }
    totrim = strspn(str, seps);
    if (totrim > 0) {
        size_t len = strlen(str);
        if (totrim == len) {
            str[0] = '\0';
        } else {
            memmove(str, str + totrim, len + 1 - totrim);
        }
    }
    return str;
}

char *rtrim(char *str, const char *seps) {
    int i;
    if (seps == NULL) {
        seps = "\t\n\v\f\r ";
    }
    i = strlen(str) - 1;
    while (i >= 0 && strchr(seps, str[i]) != NULL) {
        str[i] = '\0';
        i--;
    }
    return str;
}

char *trim(char *str, const char *seps) {
    return ltrim(rtrim(str, seps), seps);
}

void get_input(char *ServiceURL, char *message) {

  // Getting all the input
  // ask for Service URL
  printf("Please enter the serviceurl of the fiskaltrust.Service: ");
  fgets(ServiceURL, STRING_LENGTH - 1, stdin);

  // get message
  printf("Please enter the message to send in the echo request: ");
  fgets(message, STRING_LENGTH - 1, stdin);

  // trim the input strings
  trim(ServiceURL, NULL);
  trim(message, NULL);

  // if ServiceURL end with '/' -> delete it
  if (ServiceURL[strlen(ServiceURL) - 1] == '/') {
    ServiceURL[strlen(ServiceURL) - 1] = 0;
  }
}

void init_struct(struct type_Echo_request *Echo_request, char *message) {
    Echo_request->message = malloc(sizeof(char) * (strlen(message) + 1));
    strcpy(Echo_request->message, message); // set message
}

int main() {
    printf("This example sends a echo to the fiskaltrust.Service via SOAP\n");

    char ServiceURL[STRING_LENGTH];
    char message[STRING_LENGTH];

    struct type_Echo_request Echo_request;
    struct type_Echo_response Echo_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler

    get_input(ServiceURL, message);

    init_struct(&Echo_request, message);

    printf("making call... ");
    int response = soap_call_echo(ft, ServiceURL, NULL, &Echo_request, &Echo_response);
    
    if (response == SOAP_OK) {
        printf("OK\n");
        printf("Response: %s\n", Echo_response.EchoResult);
    } else {
        printf("done\n");
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    return 0;
}
