#include <BasicHttpBinding_USCOREIPOS.nsmap>
#include <inttypes.h> //int64_t, uint64_t
#include <soapH.h>
#include <stdio.h>  /* printf, sprintf */
#include <stdlib.h> /* exit, atoi, malloc, free */
#include <string.h>

#define STRING_LENGTH 256

uint64_t journal_array[] = {4707387510509010945 ,5067112530745229313};

#ifdef _WIN32
    #define type_Journal_request _ns1__Journal
    #define type_Journal_response _ns1__JournalResponse
    #define soap_call_Journal(soap, endpoint, action, Journal_request, Journal_response) soap_call___ns1__Journal(soap, endpoint, action, Journal_request, Journal_response)
#else //Linux
    #define type_Journal_request _tempuri__Journal
    #define type_Journal_response _tempuri__JournalResponse
    #define soap_call_Journal(soap, endpoint, action, Journal_request, Journal_response) soap_call___tempuri__Journal(soap, endpoint, action, Journal_request, Journal_response)
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

int64_t build_journal_type(char *journal) {
    int index;
    if(!sscanf(journal,"%d",&index)) {
        fprintf(stderr,"ERROR input is no number\n");
        exit(-1);
    }
    if(index > (sizeof(journal_array) / sizeof(uint64_t))) {
        fprintf(stderr,"ERROR invalid journal type\n");
        exit(-1);
    }
    index --;
    return journal_array[index];
}

void get_input(char *ServiceURL, char *journal) {

    // Getting all the input
    // ask for Service URL
    printf("Please enter the serviceurl of the fiskaltrust.Service: ");
    fgets(ServiceURL, STRING_LENGTH - 1, stdin);

    // get journal type
    printf("Please choose the journal you want to request\
                                                    \n1: \"AT DEP7\" \"0x4154 0000 0000 0001\"\
                                                    \n2: \"FR Ticket\" \"0x4652 0000 0000 0001\"\
                                                    \n: ");
    fgets(journal,STRING_LENGTH-1,stdin);

    // trim the input strings
    trim(ServiceURL, NULL);
    trim(journal, NULL);

    // if ServiceURL end with '/' -> delete it
    if (ServiceURL[strlen(ServiceURL) - 1] == '/') {
        ServiceURL[strlen(ServiceURL) - 1] = 0;
    }
}

void print_response(struct type_Journal_response *Journal_response) {
    printf("response:\n%s\n",Journal_response->JournalResult.__ptr);
    printf("Size: %d\n",Journal_response->JournalResult.__size);
}

void Set_request_data(struct type_Journal_request *Journal_request, int64_t journal_type) {
    
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
    char journal[STRING_LENGTH];

    struct type_Journal_request Journal_request;
    struct type_Journal_response Journal_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler

    get_input(ServiceURL, journal);

    int64_t journal_type = build_journal_type(journal);

    Set_request_data(&Journal_request, journal_type);

    printf("making call... ");
    fflush(stdout);
    int response = soap_call_Journal(ft, ServiceURL, NULL, &Journal_request, &Journal_response);
    
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
