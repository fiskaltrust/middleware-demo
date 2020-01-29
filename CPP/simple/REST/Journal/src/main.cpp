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

vector<uint64_t> journal_list = {4707387510509010945 ,5067112530745229313};

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

void get_input(string *ServiceURL, string *cashboxid, string *accesstoken, string *journal) {
    
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

    // Journal typw
    cout << "Please choose the journal you want to request\
            \n1: \"AT DEP7\" \"0x4154 0000 0000 0001\"\
            \n2: \"FR Ticket\" \"0x4652 0000 0000 0001\"\
            \n: ";
    getline(cin, *journal);

    //trim the input strings
    trim(*ServiceURL);
    trim(*cashboxid);
    trim(*accesstoken);
    trim(*journal);

    // if ServiceURL end with '/' -> delete it
    if ((*ServiceURL).back() == '/') {
        (*ServiceURL).pop_back();
    }
}

uint64_t build_journal_type(string *journal) {
    int index;
    if(!sscanf(journal->c_str(),"%d",&index)) {
        cerr << "ERROR input is no number" << endl;
        exit(-1);
    }

    if(index > journal_list.size()) {
        cerr << "ERROR invalid journal type" << endl;
        exit(-1);
    }
    index --;
    return journal_list[index];
}

void send_request(string *ServiceURL, string *cashboxid, string *accesstoken, uint64_t journal_type, string *response, int *response_code) {
    
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

    string requestURL = resault.str(4) + "/json/journal";

    //request data
    requestURL += "?type=" + to_string(journal_type) + "&from=0&to=0";
    //from and to do not have to be zero

    //set path, headers, body, Content-Type | make request
    auto res = ft->Post(requestURL.c_str(), head, "", "application/json");

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

    string ServiceURL, cashboxid, accesstoken, journal, response;

    get_input(&ServiceURL, &cashboxid, &accesstoken, &journal);

    uint64_t journal_type = build_journal_type(&journal);

    int response_code;

    send_request(&ServiceURL, &cashboxid, &accesstoken, journal_type, &response, &response_code);

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