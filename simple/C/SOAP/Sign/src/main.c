#include <BasicHttpBinding_USCOREIPOS.nsmap>
#include <soapH.h>
#include <inttypes.h> //int64_t, uint64_t
#include <stdio.h>    /* printf, sprintf */
#include <stdlib.h>   /* exit, atoi, malloc, free */
#include <string.h>
#include <time.h>

#define STRING_LENGTH 256
#define BODY_SIZE 1024

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
    for(int i = 0; target[i] != 0 ;i++) {
        if(target[i] <= 'z' && target[i] >= 'a') { target[i] &= (~(1<<5));} //the differenc from UPPER case to lower case is 32 so we unset the 5th bit 
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

void get_input(char *ServiceURL, char *cashBoxId, char *conutryCode) {

    // Getting all the input
    // ask for Service URL
    printf("Please enter the serviceurl of the fiskaltrust.Service: ");
    fgets(ServiceURL, STRING_LENGTH - 1, stdin);

    // get cashboxID
    printf("Please enter the cashbox of the fiskaltrust.Cashbox: ");
    fgets(cashBoxId, STRING_LENGTH - 1, stdin);

    // get county Code
    printf("Please enter the countyCode of the fiskaltrust.Queue: ");
    fgets(conutryCode, STRING_LENGTH - 1, stdin);

    // trim the input strings
    trim(ServiceURL, NULL);
    trim(cashBoxId, NULL);
    trim(conutryCode, NULL);

    //check countyCode length
    if(strlen(conutryCode) != 2) {
        printf("The countrycode must have length two.\n");
        exit(EXIT_FAILURE);
    }

    // if ServiceURL end with '/' -> delete it
    if (ServiceURL[strlen(ServiceURL) - 1] == '/') {
        ServiceURL[strlen(ServiceURL) - 1] = 0;
    }
}

void init_struct(struct _ns1__SignResponse *Sign_response, struct _ns1__Sign *Sign_request) {

    //allocate memory for request struct
    Sign_request->data = calloc(1, sizeof(struct ns3__ReceiptRequest));
    Sign_request->data->ftCashBoxID = calloc(128, sizeof(char));
    Sign_request->data->ftQueueID = NULL; //calloc(128, sizeof(char));
    Sign_request->data->ftPosSystemId = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbTerminalID = calloc(128, sizeof(char));
    Sign_request->data->cbReceiptReference = calloc(128, sizeof(char));
    //Sign_request->data->cbReceiptMoment muss be set bevor sending call
    Sign_request->data->cbChargeItems = calloc(1, sizeof(struct ns3__ArrayOfChargeItem));
    Sign_request->data->cbChargeItems->ChargeItem = NULL; //calloc(1, sizeof(struct ns3__ChargeItem));
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
    Sign_request->data->cbPayItems->PayItem = NULL; //calloc(1, sizeof(struct ns3__ChargeItem));
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
    Sign_request->data->ftReceiptCaseData = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbReceiptAmount = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbUser = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbArea = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbCustomer = NULL; //calloc(128, sizeof(char));
    Sign_request->data->cbSettlement = NULL; //calloc(128, sizeof(char)); 
    Sign_request->data->cbPreviousReceiptReference = NULL; //calloc(128, sizeof(char));

    //allocate memory for response struct
    Sign_response->SignResult = calloc(1, sizeof(struct ns3__ReceiptResponse));
    Sign_response->SignResult->ftCashBoxID = calloc(128, sizeof(char));
    Sign_response->SignResult->ftQueueID = calloc(128, sizeof(char));
    Sign_response->SignResult->ftQueueItemID = calloc(128, sizeof(char));
    Sign_response->SignResult->ftQueueRow = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->cbTerminalID = calloc(128, sizeof(char));
    Sign_response->SignResult->cbReceiptReference = calloc(128, sizeof(char));
    Sign_response->SignResult->ftCashBoxIdentification = calloc(128, sizeof(char));
    Sign_response->SignResult->ftReceiptIdentification = calloc(128, sizeof(char));
    //Sign_response->SignResult->cbReceiptMoment muss be set bevor sending call
    Sign_response->SignResult->ftReceiptHeader = calloc(1, sizeof(struct ns4__ArrayOfstring));
    Sign_response->SignResult->ftReceiptHeader->string = calloc(1, sizeof(char*)); //time the strings you have
    // Sign_response->SignResult->ftReceiptHeader->string[0] = calloc(128, sizeof(char)); only example

    Sign_response->SignResult->ftChargeItems = calloc(1, sizeof(struct ns3__ArrayOfChargeItem)); //times the ChargeItem you have 
    Sign_response->SignResult->ftChargeItems->ChargeItem = calloc(1, sizeof(struct ns3__ChargeItem));
    Sign_response->SignResult->ftChargeItems->ChargeItem->Position = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->ftChargeItems->ChargeItem->Quantity = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->Description = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->Amount = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->VATRate = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->ftChargeItemCase = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->ftChargeItems->ChargeItem->ftChargeItemCaseData = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->VATAmount = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->AccountNumber = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->CostCenter = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->ProductGroup = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->ProductNumber = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->ProductBarcode = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->Unit = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->UnitQuantity = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->UnitPrice = calloc(128, sizeof(char));
    Sign_response->SignResult->ftChargeItems->ChargeItem->Moment = calloc(1, sizeof(time_t));

    Sign_response->SignResult->ftChargeLines = calloc(1, sizeof(struct ns4__ArrayOfstring));
    Sign_response->SignResult->ftReceiptHeader->string = calloc(1, sizeof(char*)); //time the strings you have
    // Sign_response->SignResult->ftReceiptHeader->string[0] = calloc(128, sizeof(char)); only example

    Sign_response->SignResult->ftPayItems = calloc(1, sizeof(struct ns3__ArrayOfPayItem)); 
    Sign_response->SignResult->ftPayItems->PayItem = calloc(1, sizeof(struct ns3__ChargeItem));
    Sign_response->SignResult->ftPayItems->PayItem->Position = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->ftPayItems->PayItem->Quantity = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->Description = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->Amount = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->ftPayItemCase = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->ftPayItems->PayItem->ftPayItemCaseData = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->AccountNumber = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->CostCenter = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->MoneyGroup = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->MoneyNumber = calloc(128, sizeof(char));
    Sign_response->SignResult->ftPayItems->PayItem->Moment = calloc(1, sizeof(time_t));

    Sign_response->SignResult->ftPayLines = calloc(1, sizeof(struct ns4__ArrayOfstring));
    Sign_response->SignResult->ftReceiptHeader->string = calloc(1, sizeof(char*)); //time the strings you have
    // Sign_response->SignResult->ftReceiptHeader->string[0] = calloc(128, sizeof(char)); only example

    Sign_response->SignResult->ftReceiptIdentification = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->ftState = calloc(1, sizeof(int64_t));
    Sign_response->SignResult->ftStateData = calloc(128, sizeof(char));
}

void set_request_data(struct _ns1__Sign *Sign_request, char *cashBoxId, int64_t receiptCase) {
    //char Moment[128];
    //char Case[128];
    //time_t t = time(NULL);
    //struct tm tm = *localtime(&t);
    //sprintf(Moment,"%d-%d-%dT%d:%d:%d\n", tm.tm_year + 1900, tm.tm_mon + 1,tm.tm_mday, tm.tm_hour, tm.tm_min, tm.tm_sec);

    strcat(Sign_request->data->ftCashBoxID, cashBoxId);
    strcat(Sign_request->data->cbTerminalID, "0");
    strcat(Sign_request->data->cbReceiptReference, "0");
    Sign_request->data->cbReceiptMoment = time(NULL);
    Sign_request->data->ftReceiptCase = (int64_t)receiptCase;
}

void print_response(struct _ns1__SignResponse *Sign_response) {
    printf("Response:\n");
    printf("ftStat: %I64d",Sign_response->SignResult->ftState);
}

void free_structs(struct _ns1__SignResponse *Sign_response, struct _ns1__Sign *Sign_request) {
    //free structs
}

int main() {
    printf("This example sends a sign request to the fiskaltrust.Service via SOAP\n");

    /*
    char ServiceURL[STRING_LENGTH];
    char cashBoxId[STRING_LENGTH];
    char conutryCode[STRING_LENGTH];
    */

    char ServiceURL[] = {"http://localhost:1200/c5b315c4-0e49-46d9-8558-df475fe5c680"};
    char cashBoxId[] = {"3c44932f-5d4e-4bd0-827b-463b789f34ee"};
    char conutryCode[] = {"AT"};

    struct _ns1__Sign Sign_request;
    struct _ns1__SignResponse Sign_response;

    struct soap *ft = soap_new1(SOAP_XML_INDENT); // init handler
    

    // get_input(ServiceURL, cashBoxId, conutryCode);

    init_struct(&Sign_response, &Sign_request);

    uint64_t receiptCase = build_receipt_case(conutryCode);

    set_request_data(&Sign_request, cashBoxId, receiptCase);
    printf("%I64d\n",Sign_request.data->ftReceiptCase);
    printf("making call ...");
    int response = soap_call___ns1__Sign(ft, ServiceURL, NULL, &Sign_request, &Sign_response);
    printf("done\n");

    if (response == SOAP_OK) {
        print_response(&Sign_response);
    } else {
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    free_structs(&Sign_response, &Sign_request);

    return 0;
}