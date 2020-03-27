#include <BasicHttpBinding_USCOREIPOS.nsmap>
#include <inttypes.h> //int64_t, uint64_t
#include <soapH.h>
#include <stdio.h>  /* printf, sprintf */
#include <stdlib.h> /* exit, atoi, malloc, free */
#include <string.h>
#include <time.h>

#define STRING_LENGTH 256

#ifdef _WIN32
    #define type_Sign_request _ns1__Sign
    #define type_Sign_response _ns1__SignResponse
    #define soap_call_Sign(soap, endpoint, action, Sign_request, Sign_response) soap_call___ns1__Sign(soap, endpoint, action, Sign_request, Sign_response)
#else //Linux
    #define type_Sign_request _tempuri__Sign
    #define type_Sign_response _tempuri__SignResponse
    #define soap_call_Sign(soap, endpoint, action, Sign_request, Sign_response) soap_call___tempuri__Sign(soap, endpoint, action, Sign_request, Sign_response)
    #define ns3__ArrayOfPayItem ns1__ArrayOfPayItem
    #define ns3__ReceiptRequest ns1__ReceiptRequest
    #define ns3__ArrayOfChargeItem ns1__ArrayOfChargeItem
    #define ns3__PayItem ns1__PayItem
    #define ns3__ChargeItem ns1__ChargeItem
#endif

int64_t cases[ ][3] = {
            //{zero, start, cash}
    /*AT*/{0x4154000000000002,0x4154000000000003,0x4154000000000001},
    /*DE*/{0x4445000000000002 | 0x0000000100000000,0x4445000000000003 | 0x0000000100000000,0x444500000000001 | 0x0000000100000000}, /*pos OR implicit flag*/
    /*FR*/{0x465200000000000F,0x4652000000000010,0x4652000000000001}
};
                          //AT undefinded 10% ,DE undefinded 19% ,FR undefinded 10%
int64_t ChargeItemCase[] = {0x4154000000000001,0x4445000000000001,0x4652000000000002};

                        //AT default       ,DE default        ,FR default
int64_t PayItemCase[] = {0x4154000000000000,0x4445000000000000,0x4652000000000000};

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

void string_to_UPPERcase(char *target) {
    for (int i = 0; target[i] != 0; i++) {
        if (target[i] <= 'z' && target[i] >= 'a') {
            target[i] &= (~(1 << 5));
        } //the differenc from UPPER case to lower case is 32 so we unset the 5th bit
    }
}

void get_input(char *ServiceURL, char *cashBoxId, char *country, char *POSsystemID, int *receipt) {

    char temp[STRING_LENGTH] = {0};

    // Getting all the input
    // ask for Service URL
    printf("Please enter the serviceurl of the fiskaltrust.Service: ");
    fgets(ServiceURL, STRING_LENGTH - 1, stdin);

    // get cashboxID
    printf("Please enter the cashboxid of the fiskaltrust.Cashbox: ");
    fgets(cashBoxId, STRING_LENGTH - 1, stdin);

    // get POSsystemID
    printf("Please enter your POS System ID: ");
    fgets(POSsystemID, STRING_LENGTH - 1, stdin);

    //get country
    printf("Please enter the country of the fiskaltrust.Queue (e.g. \"AT,DE,FR\"): ");
    fgets(country,STRING_LENGTH-1,stdin);

    //get receipt case
    printf("please choose the receipt you want to send \
        \n1: zero receipt \
        \n2: start receipt \
        \n3: cash transaction \
        \n: ");
    fgets(temp,STRING_LENGTH-1,stdin);
    if(!sscanf(temp, "%d",receipt) || *receipt > 3) {
        fprintf(stderr,"ERROR wrong input\n");
        exit(-1);
    }

    // trim the input strings
    trim(ServiceURL, NULL);
    trim(cashBoxId, NULL);
    trim(country, NULL);
    trim(POSsystemID, NULL);

    string_to_UPPERcase(country);

    //check countyCode length
    if (strlen(country) != 2) {
        printf("The countrycode must have length two.\n");
        exit(EXIT_FAILURE);
    }

    // if ServiceURL end with '/' -> delete it
    if (ServiceURL[strlen(ServiceURL) - 1] == '/') {
        ServiceURL[strlen(ServiceURL) - 1] = 0;
    }
}

void set_zero_body(struct type_Sign_request *Sign_request,  char *cashBoxId, char *POSsystemID, int64_t receiptCase) {

    //allocate memory for request struct
    Sign_request->data = calloc(1, sizeof(struct ns3__ReceiptRequest));
    Sign_request->data->ftCashBoxID = calloc(128, sizeof(char));
    Sign_request->data->ftPosSystemId = calloc(128, sizeof(char));
    Sign_request->data->cbTerminalID = calloc(128, sizeof(char));
    Sign_request->data->cbReceiptReference = calloc(128, sizeof(char));
    //Sign_request->data->cbReceiptMoment muss be set before sending call
    Sign_request->data->cbChargeItems = calloc(1, sizeof(struct ns3__ArrayOfChargeItem));
    Sign_request->data->cbChargeItems->__sizeChargeItem = 1; //one empty Charge Item

    Sign_request->data->cbPayItems = calloc(1, sizeof(struct ns3__ArrayOfPayItem));
    Sign_request->data->cbPayItems->__sizePayItem = 1; //One empty Pay Item

    //Sign_request->data->ftReceiptCase;// = (int64_t)calloc(1, sizeof(int64_t));

    //Set data
    strcpy(Sign_request->data->ftCashBoxID, cashBoxId);
    strcpy(Sign_request->data->cbTerminalID, "1");
    strcpy(Sign_request->data->cbReceiptReference, "1");
    strcpy(Sign_request->data->ftPosSystemId, POSsystemID);
    Sign_request->data->cbReceiptMoment = time(NULL);
    Sign_request->data->ftReceiptCase = receiptCase;
}

void set_cash_body(struct type_Sign_request *Sign_request,  char *cashBoxId,  char *POSsystemID, int64_t receiptCase, char *country) {

    int country_index;
    if(strcmp(country, "AT") == 0) {country_index = 0;}
    else if(strcmp(country, "DE") == 0) {country_index = 1;}
    else if(strcmp(country, "FR") == 0) {country_index = 2;}

    //allocate memory for request struct
    Sign_request->data = calloc(1, sizeof(struct ns3__ReceiptRequest));
    Sign_request->data->ftCashBoxID = calloc(128, sizeof(char));
    Sign_request->data->ftPosSystemId = calloc(128, sizeof(char));
    Sign_request->data->cbTerminalID = calloc(128, sizeof(char));
    Sign_request->data->cbReceiptReference = calloc(128, sizeof(char));
    //Sign_request->data->cbReceiptMoment muss be set before sending call
    Sign_request->data->cbChargeItems = calloc(1, sizeof(struct ns3__ArrayOfChargeItem));
    Sign_request->data->cbChargeItems->__sizeChargeItem = 1; //one Charge Item
    Sign_request->data->cbChargeItems->ChargeItem = calloc(1, sizeof(struct ns3__ChargeItem));
    
    Sign_request->data->cbChargeItems->ChargeItem->Quantity = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->Description = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->Amount = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->VATRate = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->ProductNumber = calloc(128, sizeof(char));

    Sign_request->data->cbPayItems = calloc(1, sizeof(struct ns3__ArrayOfPayItem));
    Sign_request->data->cbPayItems->__sizePayItem = 1; //One Pay Item
    Sign_request->data->cbPayItems->PayItem = calloc(1, sizeof(struct ns3__PayItem));
    
    Sign_request->data->cbPayItems->PayItem->Quantity = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->Description = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->Amount = calloc(128, sizeof(char));

    // Set data
    //Receipt info
    strcpy(Sign_request->data->ftCashBoxID, cashBoxId);
    strcpy(Sign_request->data->cbTerminalID, "1");
    strcpy(Sign_request->data->cbReceiptReference, "1");
    strcpy(Sign_request->data->ftPosSystemId, POSsystemID);
    Sign_request->data->cbReceiptMoment = time(NULL);
    Sign_request->data->ftReceiptCase = receiptCase;

    //Charge Item
    strcpy(Sign_request->data->cbChargeItems->ChargeItem->Quantity, "10.0");
    strcpy(Sign_request->data->cbChargeItems->ChargeItem->Description, "Food");
    strcpy(Sign_request->data->cbChargeItems->ChargeItem->Amount, "5.0");
    strcpy(Sign_request->data->cbChargeItems->ChargeItem->VATRate, "10.0");
    strcpy(Sign_request->data->cbChargeItems->ChargeItem->ProductNumber, "1");
    Sign_request->data->cbChargeItems->ChargeItem->ftChargeItemCase =  ChargeItemCase[country_index];

    //Pay Item
    strcpy(Sign_request->data->cbPayItems->PayItem->Quantity, "10.0");
    strcpy(Sign_request->data->cbPayItems->PayItem->Description, "Cash");
    strcpy(Sign_request->data->cbPayItems->PayItem->Amount, "5.0");
    Sign_request->data->cbPayItems->PayItem->ftPayItemCase = PayItemCase[country_index];
}

int64_t get_receipt_case(char *countryCode, int receipt) {
    if(strcmp(countryCode, "AT") == 0) {return cases[0][receipt-1];}
    else if(strcmp(countryCode, "DE") == 0) {return cases[1][receipt-1];}
    else if(strcmp(countryCode, "FR") == 0) {return cases[2][receipt-1];}
    else {
        fprintf(stderr,"ERROR \"%s\" is an invalid country",countryCode);
        exit(-1);
    }
}

void print_response(struct type_Sign_response *Sign_response) {
    printf("Response:\n");
    printf("ftReceiptIdentification: %s\n",Sign_response->SignResult->ftReceiptIdentification);
    #ifdef _WIN32 
        printf("ftStat: %I64d\n", Sign_response->SignResult->ftState);
    #else
        printf("ftStat: %I64ld\n", Sign_response->SignResult->ftState);
    #endif
    for(int i = 0;i < Sign_response->SignResult->ftSignatures->__sizeSignaturItem; i++) {
        printf("SignaturItem\n");
        #ifdef _WIN32 
            printf("\tftSignatureFormat: %I64d\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureFormat);
        #else
            printf("\tftSignatureFormat: %I64ld\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureFormat);
        #endif
        #ifdef _WIN32 
            printf("\tftSignatureType: %I64d\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureType);
        #else
            printf("\tftSignatureType: %I64ld\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureType);
        #endif
        printf("\tCaption: %s\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].Caption);
        printf("\tData: %s\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].Data);
    }
    printf("ftStateData: %s\n",Sign_response->SignResult->ftStateData);
}

int main() {
    printf("This example sends a sign request to the fiskaltrust.Service via SOAP\n");

    char ServiceURL[STRING_LENGTH];
    char cashboxid[STRING_LENGTH];
    char country[STRING_LENGTH];
    char POSsystemID[STRING_LENGTH];
    int receipt;

    struct type_Sign_request Sign_request;
    struct type_Sign_response Sign_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler

    get_input(ServiceURL, cashboxid, country, POSsystemID, &receipt);

    uint64_t receip_case = get_receipt_case(country, receipt);

    if(receipt == 3) { //cash signing
        set_cash_body(&Sign_request, cashboxid, POSsystemID, receip_case, country);
    }
    else{
        set_zero_body(&Sign_request, cashboxid, POSsystemID, receip_case);
    }

    printf("making call... ");
    fflush(stdout);
    int response = soap_call_Sign(ft, ServiceURL, NULL, &Sign_request, &Sign_response);
    

    if (response == SOAP_OK) {
        printf("OK\n");
        print_response(&Sign_response);
    } else {
        printf("done\n");
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    return 0;
}