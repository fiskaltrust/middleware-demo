//#include <stdio.h> /* printf, sprintf */
//#include <stdlib.h> /* exit, atoi, malloc, free */

#include <inttypes.h> //int64_t

#include <iostream>
#include <string>

#ifndef CPPHTTPLIB_OPENSSL_SUPPORT
#define CPPHTTPLIB_OPENSSL_SUPPORT
#endif
#include "../lib/cpp-httplib/out/httplib.h"

using namespace std;
using namespace httplib;

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

void get_input(string *ServiceURL, string *cashboxid, string *accesstoken, string *body) {
    
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

    // get message
    cout << "Please enter the message to send in the echo request: ";
    getline(cin, *body);

    //trim the input strings
    trim(*ServiceURL);
    trim(*cashboxid);
    trim(*accesstoken);
    trim(*body);

    // if ServiceURL end with '/' -> delete it
    if ((*ServiceURL).back() == '/') {
        (*ServiceURL).pop_back();
    }

    // if ServiceURL start with 'https://' -> delete it (library demands the URL without 'https://')
    if((*ServiceURL).find("https://",8)) { (*ServiceURL).erase(0,8); }
    else if((*ServiceURL).find("http://",7)) { (*ServiceURL).erase(0,7); }

    //add " to beginning and end of string
    *body = "\"" + *body + "\"";
}

void send_request(string *ServiceURL, string *cashboxid, string *accesstoken, string *body, string *response, int *response_code) {
    
    Headers head = {
        //{"Content-Type", "application/json"},
        {"cashboxid", cashboxid->c_str()},
        {"accesstoken", accesstoken->c_str()}
    };

    //string requestURL = *ServiceURL + "/json/echo";
    SSLClient ft("signaturcloud-sandbox.fiskaltrust.at");
    ft.set_follow_location(true);

    cout << "performing request... ";

    auto res = ft.Post("/json/echo", head, *body, "application/json");

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
    cout << "This example sends an echo request to the fiskaltrust.Service via REST" << endl;

    string ServiceURL ("signaturcloud-sandbox.fiskaltrust.at");
    string cashboxid ("a37ce376-62be-42c6-b560-1aa0a6700211");
    string accesstoken ("BJ6ZufH6hcCHmu2yzc9alH45FjdlCUT1YDlAf83gTydHKj1ZWcMibPlheky1WLMc+E9WeHYanQ8vS5oCirhI6Ck=");
    string body ("\"some\"");
    string response;

    get_input(&ServiceURL, &cashboxid, &accesstoken, &body);

    int response_code;

    send_request(&ServiceURL, &cashboxid, &accesstoken, &body, &response, &response_code);

    //print Response
    if(response.empty()) {
        cout << "No Response" << endl;
    }
    else {
        cout << "Response Code: " << response_code << endl;
        cout << "Body:" << endl << response << endl;
    }
    return 0;
}