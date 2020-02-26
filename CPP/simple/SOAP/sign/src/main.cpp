#include <BasicHttpBinding_USCOREIPOS.nsmap>
#ifdef _WIN32
    #include <soapBasicHttpBinding_USCOREIPOSProxy.h>
#else
    #include <soapH.h>
#endif
#include <inttypes.h> //int64_t, uint64_t
//#include <stdio.h>    /* printf, sprintf */
//#include <stdlib.h>   /* exit, atoi, malloc, free */
#include <iostream>
#include <algorithm> // for_each
#include <string>
#include <vector>

using namespace std;

#ifdef _WIN32
    #define type_Sign_request _ns1__Sign
    #define type_Sign_response _ns1__SignResponse
    #define type_ReceiptRequest ns3__ReceiptRequest
    #define type_ChargeItem ns3__ChargeItem
    #define type_ArrayOfChargeItem ns3__ArrayOfChargeItem
    #define type_PayItem ns3__PayItem
    #define type_ArrayOfPayItem ns3__ArrayOfPayItem

    #define new_Sign_request soap_new_set__ns1__Sign
    #define soap_Sign_call soap_call___ns1__Sign
    #define soap_new_ReceiptRequest soap_new_req_ns3__ReceiptRequest
    #define soap_new_ArrayOfChargeItem soap_new_ns3__ArrayOfChargeItem
    #define soap_new_ArrayOfPayItem soap_new_ns3__ArrayOfPayItem
    #define soap_new_req_ChargeItem soap_new_req_ns3__ChargeItem
    #define soap_new_ArrayOfChargeItem soap_new_ns3__ArrayOfChargeItem
    #define soap_new_req_PayItem soap_new_req_ns3__PayItem
    #define soap_new_ArrayOfPayItem soap_new_ns3__ArrayOfPayItem
#else //Linux
    #define type_Sign_request _tempuri__Sign
    #define type_Sign_response _tempuri__SignResponse
    #define type_ReceiptRequest ns1__ReceiptRequest
    #define type_ChargeItem ns1__ChargeItem
    #define type_ArrayOfChargeItem ns1__ArrayOfChargeItem
    #define type_PayItem ns1__PayItem
    #define type_ArrayOfPayItem ns1__ArrayOfPayItem

    #define new_Sign_request soap_new_set__tempuri__Sign
    #define soap_Sign_call soap_call___tempuri__Sign
    #define soap_new_ReceiptRequest soap_new_req_ns1__ReceiptRequest
    #define soap_new_ArrayOfChargeItem soap_new_ns1__ArrayOfChargeItem
    #define soap_new_ArrayOfPayItem soap_new_ns1__ArrayOfPayItem
    #define soap_new_req_ChargeItem soap_new_req_ns1__ChargeItem
    #define soap_new_ArrayOfChargeItem soap_new_ns1__ArrayOfChargeItem
    #define soap_new_req_PayItem soap_new_req_ns1__PayItem
    #define soap_new_ArrayOfPayItem soap_new_ns1__ArrayOfPayItem
#endif

vector<vector<int64_t>> cases = {
            //{zero, start, cash}
    /*AT*/{0x4154000000000002,0x4154000000000003,0x4154000000000001},
    /*DE*/{0x4445000000000002,0x4445000000000003,/*pos OR implicit flag*/0x444500000000001 | 0x0000000100000000},
    /*FR*/{0x465200000000000F,0x4652000000000010,0x4652000000000001}
};
                                //AT undefinded 10% ,DE undefinded 19% ,FR undefinded 10%
vector<int64_t> ChargeItemCase = {0x4154000000000001,0x4445000000000001,0x4652000000000002};

                              //AT default       ,DE default        ,FR default
vector<int64_t> PayItemCase = {0x4154000000000000,0x4445000000000000,0x4652000000000000};

string cbTerminalID = "1";
string cbReceiptReference = "1";

string ChargeItem_Quantity = "10.0";
string ChargeItem_Description = "Food";
string ChargeItem_Amount = "5.0";
string ChargeItem_VATRate = "10.0";

string PayItem_Quantity = "10.0";
string PayItem_Description = "Cash";
string PayItem_Amount = "5.0";

std::string &ltrim(std::string &str, const std::string &chars = "\t\n\v\f\r ") {
    str.erase(0, str.find_first_not_of(chars));
    return str;
}

std::string &rtrim(std::string &str, const std::string &chars = "\t\n\v\f\r ") {
    str.erase(str.find_last_not_of(chars) + 1);
    return str;
}

std::string &trim(std::string &str, const std::string &chars = "\t\n\v\f\r ") {
    return ltrim(rtrim(str, chars), chars);
}

void get_input(string *ServiceURL, string *cashboxid, string *country, string *POSSID, int *receipt) {

    string temp;
    // Getting all the input
    // ask for Service URL
    cout << "Please enter the serviceurl of the fiskaltrust.Service: ";
    getline(cin, *ServiceURL);

    //get cashboxID
    cout << "Please enter the cashboxid of the fiskaltrust.CashBox: ";
    getline(cin, *cashboxid);

    // ask for Service POSSID
    cout << "Please enter the POS System ID of the fiskaltrust.Service: ";
    getline(cin, *POSSID);

    // country code
    cout << "Please enter the country of the fiskaltrust.Queue (e.g. \"AT,DE,FR\"): ";
    getline(cin, *country);

    // get message
    cout << "please choose the receipt you want to send \
        \n1: zero receipt \
        \n2: start receipt \
        \n3: cash transaction \
        \n: ";
    getline(cin, temp);
    if(!sscanf(temp.c_str(), "%d",receipt) || *receipt > 3) {
        cerr << "ERROR wrong input" << endl;
        exit(-1);
    }

    // trim the input strings
    trim(*ServiceURL);
    trim(*POSSID);

    // if ServiceURL end with '/' -> delete it
    if ((*ServiceURL).back() == '/') {
        (*ServiceURL).pop_back();
    }
}

int get_conuty_index(string country) {
    for_each(country.begin(), country.end(), [](char & c){
	    c = ::toupper(c);
    });

    if(country == "AT") {return 0;}
    if(country == "DE") {return 1;}
    if(country == "FR") {return 2;}

    cerr << "ERROR unknow country" << endl;
    exit(-1);
}

type_Sign_request *build_zero_body(struct soap *ft, string *cashboxid, string *POSSID, string country, int receipt) {

    //create request data class
    type_ReceiptRequest *data = soap_new_ReceiptRequest(ft,
            cashboxid,
            &cbTerminalID,
            &cbReceiptReference,
            time(nullptr),
            soap_new_ArrayOfChargeItem(ft),
            soap_new_ArrayOfPayItem(ft),
            cases[get_conuty_index(country)][receipt-1]
            );
    
    data->ftPosSystemId = POSSID;
    
    return new_Sign_request(ft, data);
}

type_Sign_request *build_cash_body(struct soap *ft, string *cashboxid, string *POSSID, string country, int receipt) {
    int conuty_index = get_conuty_index(country);
    //create cbChargeItems array
    //create ChargeItem

    vector<type_ChargeItem*> ChargeItem = { 
        new type_ChargeItem
    };

    ChargeItem[0]->Quantity = ChargeItem_Quantity;
    ChargeItem[0]->Description = &ChargeItem_Description;
    ChargeItem[0]->Amount = ChargeItem_Amount;
    ChargeItem[0]->VATRate = ChargeItem_VATRate;
    ChargeItem[0]->ftChargeItemCase = ChargeItemCase[conuty_index];
    
    //add item to array
    type_ArrayOfChargeItem *ChargeItems = soap_new_ArrayOfChargeItem(ft);
    ChargeItems->ChargeItem = ChargeItem;
    
    //create cbPayItems array
    //creat cbPayItem

    vector<type_PayItem*> PayItem = { new type_PayItem};

    PayItem[0]->Quantity = PayItem_Quantity;
    PayItem[0]->Description = &PayItem_Description;
    PayItem[0]->Amount = PayItem_Amount;
    PayItem[0]->ftPayItemCase = PayItemCase[conuty_index];

    //add item to array
    type_ArrayOfPayItem *PayItems = soap_new_ArrayOfPayItem(ft);
    PayItems->PayItem = PayItem;
    
    //create request data class
    type_ReceiptRequest *data = soap_new_ReceiptRequest(ft,
            cashboxid,
            &cbTerminalID,
            &cbReceiptReference,
            time(nullptr),
            ChargeItems,
            PayItems,
            cases[conuty_index][receipt-1]
            );

    data->ftPosSystemId = POSSID;

    return new_Sign_request(ft, data);
}

void print_response(type_Sign_response Sign_response, struct soap *ft) {
    if(Sign_response.SignResult->ftSignatures) { 
        for(std::vector<int>::size_type i = 0; i != Sign_response.SignResult->ftSignatures->SignaturItem.size(); i++) {
            cout << "Signature" << endl;
            cout << "\tftSignatureFormat \"" << Sign_response.SignResult->ftSignatures->SignaturItem[i]->ftSignatureFormat << "\"" << endl;
            cout << "\tftSignatureType \"" << Sign_response.SignResult->ftSignatures->SignaturItem[i]->ftSignatureType << "\"" << endl;
            cout << "\tCaption \"" << Sign_response.SignResult->ftSignatures->SignaturItem[i]->Caption << "\"" << endl;
            cout << "\tData \"" << Sign_response.SignResult->ftSignatures->SignaturItem[i]->Data << "\"" << endl;
        }
        
    }
    cout << "ftState: " << Sign_response.SignResult->ftState << endl;
    cout << "ftStateData: " << (*Sign_response.SignResult->ftStateData) << endl;
}

int main() {
    cout << "This example sends a echo to the fiskaltrust.Service via SOAP\n";

    string ServiceURL, cashboxid, country, POSSID;
    int receipt;
    
    //ServiceURL = "http://localhost:1200/c5b315c4-0e49-46d9-8558-df475fe5c680";
    //cashboxid = "3c44932f-5d4e-4bd0-827b-463b789f34ee";
    //country = "AT";
    //POSSID = "some id";
    //receipt = 3;
    
    get_input(&ServiceURL, &cashboxid, &country, &POSSID, &receipt);

    //init echo class
    struct soap *ft = soap_new();

    class type_Sign_request *Sign_request;
    class type_Sign_response Sign_response;

    if(receipt == 3) {
        Sign_request = build_cash_body(ft, &cashboxid, &POSSID, country, receipt);
    }
    else {
        Sign_request = build_zero_body(ft, &cashboxid, &POSSID, country, receipt);
    }

    cout << "Quantity: " << Sign_request->data->cbChargeItems->ChargeItem[0]->Quantity << endl;
    cout << "case: " << Sign_request->data->cbPayItems->PayItem[0]->ftPayItemCase << endl;

    //make call
    cout << "making call... ";

    if(soap_Sign_call(ft,ServiceURL.c_str(),NULL, Sign_request, Sign_response) == SOAP_OK) {
        cout << "OK" << endl;
        cout << "Response: " << endl;
        print_response(Sign_response, ft);
    } else {
        cout << "done" << endl;
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context

    return 0;
}
