#include <BasicHttpBinding_USCOREIPOS.nsmap>
#include <inttypes.h> //int64_t, uint64_t
#include <soapH.h>
#include <stdio.h>  /* printf, sprintf */
#include <stdlib.h> /* exit, atoi, malloc, free */
#include <string.h>

#define STRING_LENGTH 256

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

int64_t build_Journal_type(char *countryCode) {
    int64_t receipt_case = 0;

    //add county Code
    receipt_case |= ((int64_t)countryCode[0] << (4 * 14));
    receipt_case |= ((int64_t)countryCode[1] << (4 * 12));

    //zero receipt
    receipt_case |= 1;

    return receipt_case;
}

void get_input(char *ServiceURL, char *conutryCode) {

    // Getting all the input
    // ask for Service URL
    printf("Please enter the serviceurl of the fiskaltrust.Service: ");
    fgets(ServiceURL, STRING_LENGTH - 1, stdin);

    // get county Code
    printf("Please enter the countyCode of the fiskaltrust.Queue: ");
    fgets(conutryCode, STRING_LENGTH - 1, stdin);

    // trim the input strings
    trim(ServiceURL, NULL);
    trim(conutryCode, NULL);

    //check countyCode length
    if (strlen(conutryCode) != 2) {
        printf("The countrycode must have length two.\n");
        exit(EXIT_FAILURE);
    }

    // if ServiceURL end with '/' -> delete it
    if (ServiceURL[strlen(ServiceURL) - 1] == '/') {
        ServiceURL[strlen(ServiceURL) - 1] = 0;
    }
}

void print_response(struct _ns1__JournalResponse *Journal_response) {
    if(strlen(Journal_response->JournalResult.__ptr) > 2000) {
        printf("ptr: %.1000s\n",Journal_response->JournalResult.__ptr);
    printf("\t*\n\t*\n\t*\n\t*\n\t*\n%s\n",Journal_response->JournalResult.__ptr + (strlen(Journal_response->JournalResult.__ptr) - 1000));
    }
    else {printf("ptr: %s\n",Journal_response->JournalResult.__ptr);}
    printf("Size: %d\n",Journal_response->JournalResult.__size);
}

void Set_request_data(struct _ns1__Journal *Journal_request, int64_t journal_type) {
    
    Journal_request->ftJournalType = malloc(sizeof(int64_t));
    *(Journal_request->ftJournalType) = journal_type;
    Journal_request->from = malloc(sizeof(int64_t));
    *(Journal_request->from) = 0;
    Journal_request->to = malloc(sizeof(int64_t));
    *(Journal_request->to) = 0;
}

int main() {
    printf("This example sends a sign request to the fiskaltrust.Service via SOAP\n");

    char ServiceURL[STRING_LENGTH];
    char countycode[STRING_LENGTH];

    struct _ns1__Journal Journal_request;
    struct _ns1__JournalResponse Journal_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler

    get_input(ServiceURL, countycode);

    int64_t journal_type = build_Journal_type(countycode);

    Set_request_data(&Journal_request, journal_type);

    printf("making call... ");
    int response = soap_call___ns1__Journal(ft, ServiceURL, NULL, &Journal_request, &Journal_response);
    
    if (response == SOAP_OK) {
        // Print response
        printf("OK\n");
        print_response(&Journal_response);
    } else {
        printf("done\n");
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    return 0;
}
