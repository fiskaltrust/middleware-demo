//#include <stdio.h> /* printf, sprintf */
//#include <stdlib.h> /* exit, atoi, malloc, free */

#include <inttypes.h> //int64_t

#ifndef CPPHTTPLIB_OPENSSL_SUPPORT
#define CPPHTTPLIB_OPENSSL_SUPPORT
#endif
#include <httplib.h>
#include <nlohmann/json.hpp>

#include <iostream>
#include <algorithm> // for_each
#include <string>
#include <vector>
#include <regex>

using namespace std;
using namespace httplib;
using json = nlohmann::json;

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

void string_to_UPPERcase(char *target) {
    for(int i = 0; target[i] != 0 ;i++) {
        if(target[i] <= 'z' && target[i] >= 'a') { target[i] &= (~(1<<5));} //the differenc from UPPER case to lower case is 32 so we unset the 5th bit 
    }
}

void get_input(string *ServiceURL, string *cashboxid, string *accesstoken, string *country, string *POSSID, int *receipt) {
    
    string temp;
    //Getting all the input
    //ask for Service URL
    cout << "Please enter the serviceurl of the fiskaltrust.Service: ";
    getline(cin, *ServiceURL);

    //get cashboxID
    cout << "Please enter the cashboxid of the fiskaltrust.CashBox: ";
    getline(cin, *cashboxid);

    //get accesstoken
    cout << "Please enter the accesstoken of the fiskaltrust.CashBox: ";
    getline(cin, *accesstoken);

    //get POS system ID
    cout << "Please enter the POS system ID of the fiskaltrust.CashBox: ";
    getline(cin, *POSSID);

    // country code
    cout << "Please enter the country of the fiskaltrust.Queue (e.g. \"AT,DE,FR\"): ";
    getline(cin, *country);

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

    //trim the input strings
    trim(*ServiceURL);
    trim(*cashboxid);
    trim(*accesstoken);
    trim(*country);

    // if ServiceURL end with '/' -> delete it
    if ((*ServiceURL).back() == '/') {
        (*ServiceURL).pop_back();
    }
}

int get_conuty_index(string country) {
    std::for_each(country.begin(), country.end(), [](char & c){
	    c = ::toupper(c);
    });

    if(country == "AT") {return 0;}
    if(country == "DE") {return 1;}
    if(country == "FR") {return 2;}

    cerr << "ERROR unknow country" << endl;
    exit(-1);
}

json build_zero_body(string cashboxid, string country, string POSSID, int receipt) {
    string timestamp = "/Date(" + to_string(time(nullptr)) + ")/";
    json root;
    root["ftCashBoxID"] = cashboxid;
    root["ftPosSystemId"] = POSSID;
    root["cbTerminalID"] = "1";
    root["cbReceiptReference"] = "1";
    root["cbReceiptMoment"] = timestamp;
    root["cbChargeItems"] = json::array();
    root["cbPayItems"] = json::array();
    root["ftReceiptCase"] = cases[get_conuty_index(country)][receipt-1];
    return root;
}

json build_cash_body(string cashboxid, string country, string POSSID, int receipt) {
    string timestamp = "/Date(" + to_string(time(nullptr)) + ")/";
    int conuty_index = get_conuty_index(country);

    //create cbChargeItems array
    //create ChargeItem
    json ChargeItem;
    ChargeItem["Quantity"] = 10.0;
    ChargeItem["Description"] = "Food";
    ChargeItem["Amount"] = 5.0;
    ChargeItem["VATRate"] = 10.0;
    ChargeItem["ftChargeItemCase"] = ChargeItemCase[conuty_index];
    ChargeItem["ProductNumber"] = "1";

    //add item to array
    json ChargeItems = json::array();
    ChargeItems.push_back(ChargeItem);

    //create cbPayItems array
    //creat cbPayItem
    json PayItem;
    PayItem["Quantity"] = 10.0;
    PayItem["Description"] = "Cash";
    PayItem["Amount"] = 5.0;
    PayItem["ftPayItemCase"] = PayItemCase[conuty_index];

    //add item to array
    json PayItems = json::array();
    PayItems.push_back(PayItem);

    //create root object
    json root;
    root["ftCashBoxID"] = cashboxid;
    root["ftPosSystemId"] = POSSID;
    root["cbTerminalID"] = "1";
    root["cbReceiptReference"] = "1";
    root["cbReceiptMoment"] = timestamp;
    root["cbChargeItems"] = ChargeItems;
    root["cbPayItems"] = PayItems;
    root["ftReceiptCase"] = cases[conuty_index][receipt-1];

    return root;
}

void send_request(string *ServiceURL, string *cashboxid, string *accesstoken, string body, string *response, int *response_code) {
    
    Client *ft;
    Headers head = {
        {"cashboxid", cashboxid->c_str()},
        {"accesstoken", accesstoken->c_str()}
    };

    regex expression("^(https?):\\/\\/([^\\/:]*)(?::([0-9]+))?(\\/.*)?$");
    smatch resault;

    //get parts of the Service URL
    //http://localhost:1200/test
    //| 1 |  |   2    || 3 |  4
    regex_search(*ServiceURL, resault ,expression);

    if(resault.str(1) == "http") { //unsecure Client
        ft = new Client(resault.str(2), stoi(resault.str(3)));
    }
    else if(resault.str(1) == "https") { //secure Client
        ft = new SSLClient(resault.str(2));
    }
    else {
        cerr << "ERROR not supported protocol" << endl;
    }
    
    ft->set_follow_location(true); 

    cout << "performing request... ";
    cout.flush();

    string requestURL = resault.str(4) + "/json/sign";

    //set path, headers, body, Content-Type | make request
    auto res = ft->Post(requestURL.c_str(), head, body, "application/json");

    if (res) {
        cout << "OK" << endl;
        *response_code = res->status;
        *response = res->body;
    }
    else {
        cout << "failed" << endl;
        cerr << "ERROR connection failed" << endl;
    }
    
}

int main()
{
    cout << "This example sends a sign request to the fiskaltrust.Service via REST" << endl;

    string ServiceURL, cashboxid, accesstoken, country_code, POSSID, response_body;

    int response_code, receipt;

    json request, response;

    get_input(&ServiceURL, &cashboxid, &accesstoken, &country_code, &POSSID, &receipt);

    if(receipt == 3 ) {
        request = build_cash_body(cashboxid, country_code, POSSID, receipt);
    }
    else {
        request = build_zero_body(cashboxid, country_code, POSSID, receipt);
    }

    send_request(&ServiceURL, &cashboxid, &accesstoken, request.dump(), &response_body, &response_code);

    //print Response
    if(response_body.empty()) {
        cout << "No Response" << endl;
    }
    else {
        cout << "Response Code: " << response_code << endl;
        //Print response
        response = json::parse(response_body);
        cout << response.dump(4) << endl;
        if(response.count("ftState")) { //check if enty is available
            cout << "ftState: " << response.at("/ftState"_json_pointer) << endl;
        }
    }

    return 0;
}