#include <inttypes.h> //int64_t, uint64_t
#include <stdio.h>    /* printf, sprintf */
#include <stdlib.h>   /* exit, atoi, malloc, free */
#include <string.h>
//#include "ftservice.h"
#include "soapH.h"
//#include "soapStub.h"
#include "BasicHttpBinding_USCOREIPOS.nsmap"

#define STRING_LENGTH 256
#define BODY_SIZE 1024

#define DEBUG
#define CLOUD

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

    // get cashboxID
    printf("Please enter the messaeg to send in the echo request: ");
    fgets(message, STRING_LENGTH - 1, stdin);

    // trim the input strings
    trim(ServiceURL, NULL);
    trim(message, NULL);

    // if ServiceURL end with '/' -> delete it
    if (ServiceURL[strlen(ServiceURL) - 1] == '/') {
        ServiceURL[strlen(ServiceURL) - 1] = 0;
    }
}

void init_struct(struct _ns1__EchoResponse *Echo_response, struct _ns1__Echo *Echo_request, char *message) {
    Echo_request->message = malloc(sizeof(char) * (strlen(message) + 1));

    Echo_response->EchoResult =
        malloc(sizeof(char) * 1024); // we dont know what will come back

    strcat(Echo_request->message, message); // set message
}

int main() {
    printf("This example sends a sign request to the fiskaltrust.Service via SOAP\n");

    /*
  char ServiceURL[STRING_LENGTH];
  char message[STRING_LENGTH];
  */

    char ServiceURL[] = {"http://localhost:1200/c5b315c4-0e49-46d9-8558-df475fe5c680"};
    char message[] = {"test_message"};

    struct _ns1__Echo Echo_request;
    struct _ns1__EchoResponse Echo_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler

    // get_input(ServiceURL, message);

    init_struct(&Echo_response, &Echo_request, message);

    printf("making call ...");
    int response = soap_call___ns1__Echo(ft, ServiceURL, NULL, &Echo_request, &Echo_response);
    printf("done response: %d\n", ft->error);

    // int response = soap_send___ns1__Echo(ft, ServiceURL, NULL, &Echo_request);
    // printf("Send Response: %d\n",ft->error);

    if (response == SOAP_OK) {

        // soap_recv___ns1__Echo(ft, &Echo_response);
        // printf("recv Response: %d\n",ft->error);
        printf("Response: %s\n", Echo_response.EchoResult);
        // soap_print_fault(ft, stderr);
    } else {
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    free(Echo_request.message);
    free(Echo_response.EchoResult);

    return 0;
}
