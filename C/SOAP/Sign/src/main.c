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
#endif

int64_t cases[ ][3] = {
            //{zero, start, cash}
    /*AT*/{0x4154000000000002,0x4154000000000003,0x4154000000000001},
    /*DE*/{0x4445000000000002,0x4445000000000003,/*pos OR implicit flag*/0x444500000000001 | 0x0000000100000000},
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

int64_t build_receipt_case(char *countryCode) {
    int64_t receipt_case = 0;

    //add county Code
    receipt_case |= ((int64_t)countryCode[0] << (4 * 14));
    receipt_case |= ((int64_t)countryCode[1] << (4 * 12));

    //zero receipt
    receipt_case |= 2;

    return receipt_case;
}

void get_input(char *ServiceURL, char *cashBoxId, char *country, int *receipt) {

    char temp[STRING_LENGTH] = {0};

    // Getting all the input
    // ask for Service URL
    printf("Please enter the serviceurl of the fiskaltrust.Service: ");
    fgets(ServiceURL, STRING_LENGTH - 1, stdin);

    // get cashboxID
    printf("Please enter the cashbox of the fiskaltrust.Cashbox: ");
    fgets(cashBoxId, STRING_LENGTH - 1, stdin);

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

void set_zero_body(struct type_Sign_request *Sign_request,  char *cashBoxId, int64_t receiptCase) {

    //allocate memory for request struct
    Sign_request->data = calloc(1, sizeof(struct ns3__ReceiptRequest));
    Sign_request->data->ftCashBoxID = calloc(128, sizeof(char));
    Sign_request->data->ftQueueID = NULL;
    Sign_request->data->ftPosSystemId = NULL;
    Sign_request->data->cbTerminalID = calloc(128, sizeof(char));
    Sign_request->data->cbReceiptReference = calloc(128, sizeof(char));
    //Sign_request->data->cbReceiptMoment muss be set bevor sending call
    Sign_request->data->cbChargeItems = calloc(1, sizeof(struct ns3__ArrayOfChargeItem));
    Sign_request->data->cbChargeItems->__sizeChargeItem = 1; //one empty Charge Item
    Sign_request->data->cbChargeItems->ChargeItem = NULL;

    Sign_request->data->cbPayItems = calloc(1, sizeof(struct ns3__ArrayOfPayItem));
    Sign_request->data->cbPayItems->__sizePayItem = 1; //One empty Pay Item
    Sign_request->data->cbPayItems->PayItem = NULL;

    Sign_request->data->ftReceiptCase = calloc(1, sizeof(int64_t));
    Sign_request->data->ftReceiptCaseData = NULL;          
    Sign_request->data->cbReceiptAmount = NULL;           
    Sign_request->data->cbUser = NULL;                    
    Sign_request->data->cbArea = NULL;                    
    Sign_request->data->cbCustomer = NULL;                
    Sign_request->data->cbSettlement = NULL;              
    Sign_request->data->cbPreviousReceiptReference = NULL;

    //Set data
    strcpy(Sign_request->data->ftCashBoxID, cashBoxId);
    strcpy(Sign_request->data->cbTerminalID, "1");
    strcpy(Sign_request->data->cbReceiptReference, "1");
    Sign_request->data->cbReceiptMoment = time(NULL);
    Sign_request->data->ftReceiptCase = receiptCase;

    //Response struct will be allocated by the call function
}

void set_cash_body(struct type_Sign_request *Sign_request,  char *cashBoxId, int64_t receiptCase, char *country) {

    //allocate memory for request struct
    Sign_request->data = calloc(1, sizeof(struct ns3__ReceiptRequest));
    Sign_request->data->ftCashBoxID = calloc(128, sizeof(char));
    Sign_request->data->ftQueueID = NULL;     //calloc(128, sizeof(char));
    Sign_request->data->ftPosSystemId = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbTerminalID = calloc(128, sizeof(char));
    Sign_request->data->cbReceiptReference = calloc(128, sizeof(char));
    //Sign_request->data->cbReceiptMoment muss be set bevor sending call
    Sign_request->data->cbChargeItems = calloc(1, sizeof(struct ns3__ArrayOfChargeItem));
    Sign_request->data->cbChargeItems->__sizeChargeItem = 1; //one empty Charge Item
    Sign_request->data->cbChargeItems->ChargeItem = NULL;    //calloc(1, sizeof(struct ns3__ChargeItem));
    /*
    Sign_request->data->cbChargeItems->ChargeItem->Position = calloc(1, sizeof(int64_t));
    Sign_request->data->cbChargeItems->ChargeItem->Quantity = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->Description = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->Amount = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->VATRate = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->ftChargeItemCase = calloc(1, sizeof(int64_t));
    Sign_request->data->cbChargeItems->ChargeItem->ftChargeItemCaseData = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->VATAmount = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->AccountNumber = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->CostCenter = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->ProductGroup = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->ProductNumber = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->ProductBarcode = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->Unit = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->UnitQuantity = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->UnitPrice = calloc(128, sizeof(char));
    Sign_request->data->cbChargeItems->ChargeItem->Moment = calloc(1, sizeof(time_t));
    */

    Sign_request->data->cbPayItems = calloc(1, sizeof(struct ns3__ArrayOfPayItem));
    Sign_request->data->cbPayItems->__sizePayItem = 1; //One empty Pay Item
    Sign_request->data->cbPayItems->PayItem = NULL;    //calloc(1, sizeof(struct ns3__ChargeItem));
    /*
    Sign_request->data->cbPayItems->PayItem->Position = calloc(1, sizeof(int64_t));
    Sign_request->data->cbPayItems->PayItem->Quantity = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->Description = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->Amount = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->ftPayItemCase = calloc(1, sizeof(int64_t));
    Sign_request->data->cbPayItems->PayItem->ftPayItemCaseData = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->AccountNumber = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->CostCenter = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->MoneyGroup = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->MoneyNumber = calloc(128, sizeof(char));
    Sign_request->data->cbPayItems->PayItem->Moment = calloc(1, sizeof(time_t));
    */

    Sign_request->data->ftReceiptCase = calloc(1, sizeof(int64_t));
    Sign_request->data->ftReceiptCaseData = NULL;          //calloc(128, sizeof(char));
    Sign_request->data->cbReceiptAmount = NULL;            //calloc(128, sizeof(char));
    Sign_request->data->cbUser = NULL;                     //calloc(128, sizeof(char));
    Sign_request->data->cbArea = NULL;                     //calloc(128, sizeof(char));
    Sign_request->data->cbCustomer = NULL;                 //calloc(128, sizeof(char));
    Sign_request->data->cbSettlement = NULL;               //calloc(128, sizeof(char));
    Sign_request->data->cbPreviousReceiptReference = NULL; //calloc(128, sizeof(char));

    //Response struct will be allocated by the call function
}

void set_request_data(struct type_Sign_request *Sign_request, char *cashBoxId, int64_t receiptCase) {

    strcat(Sign_request->data->ftCashBoxID, cashBoxId);
    strcat(Sign_request->data->cbTerminalID, "1");
    strcat(Sign_request->data->cbReceiptReference, "1");
    Sign_request->data->cbReceiptMoment = time(NULL);
    Sign_request->data->ftReceiptCase = (int64_t)receiptCase;
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
        printf("ftStat: %I64lld\n", Sign_response->SignResult->ftState);
    #endif
    for(int i = 0;i < Sign_response->SignResult->ftSignatures->__sizeSignaturItem; i++) {
        printf("SignaturItem\n");
        #ifdef _WIN32 
            printf("\tftSignatureFormat: %I64d\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureFormat);
        #else
            printf("\tftSignatureFormat: %I64lld\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureFormat);
        #endif
        #ifdef _WIN32 
            printf("\tftSignatureType: %I64d\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureType);
        #else
            printf("\tftSignatureType: %I64lld\n",Sign_response->SignResult->ftSignatures->SignaturItem[i].ftSignatureType);
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
    int receipt;

    struct type_Sign_request Sign_request;
    struct type_Sign_response Sign_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler

    get_input(ServiceURL, cashboxid, country, &receipt);

    uint64_t receip_case = build_receipt_case(country, receipt);

    if(receipt == 3) { //cash signing
        set_cash_body(&Sign_request, cashboxid, receip_case, country);
    }
    else{
        set_zero_body(&Sign_request, cashboxid, receip_case);
    }

    printf("making call... ");
    fflush(stdout);
    int response = soap_call_Sign(ft, ServiceURL, NULL, &Sign_request, &Sign_response);
    printf("done\n");

    if (response == SOAP_OK) {
        print_response(&Sign_response);
    } else {
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    return 0;
}