//#include <stdio.h> /* printf, sprintf */
//#include <stdlib.h> /* exit, atoi, malloc, free */

#include <inttypes.h> //int64_t

#ifndef CPPHTTPLIB_OPENSSL_SUPPORT
#define CPPHTTPLIB_OPENSSL_SUPPORT
#endif
#include <httplib.h>

#include <iostream>
#include <string>
#include <regex>

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

    //add " to beginning and end of string
    *body = "\"" + *body + "\"";
}

void send_request(string *ServiceURL, string *cashboxid, string *accesstoken, string *body, string *response, int *response_code) {
    
    Client *ft;
    Headers head = {
        {"cashboxid", cashboxid->c_str()},
        {"accesstoken", accesstoken->c_str()}
    };

    regex expression("^(https?):\\/\\/([^\\/:]*)(?::([0-9]+))?(\\/.*)?$");
    smatch resault;

    regex_search(*ServiceURL, resault ,expression);

    if(resault.str(1) == "http") { //http Client
        ft = new Client(resault.str(2), stoi(resault.str(3)));
    }
    else if(resault.str(1) == "https") {
        ft = new SSLClient(resault.str(2));
    }
    else {
        cerr << "ERROR not supported protocol" << endl;
    }
    
    ft->set_follow_location(true);

    cout << "performing request... ";
    cout.flush();

    string requestURL = resault.str(4) + "/json/echo";

    auto res = ft->Post(requestURL.c_str(), head, *body, "application/json");

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

    string ServiceURL, cashboxid, accesstoken, body, response;

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