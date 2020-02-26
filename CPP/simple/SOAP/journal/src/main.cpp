#include <BasicHttpBinding_USCOREIPOS.nsmap>
#ifdef _WIN32
    #include <soapBasicHttpBinding_USCOREIPOSProxy.h>
#else
    #include <soapH.h>
#endif
#include <inttypes.h> //int64_t, uint64_t
#include <iostream>
#include <string>
#include <vector>

using namespace std;

#ifdef _WIN32
#define type_journal_request _ns1__Journal
#define type_journal_response _ns1__JournalResponse
#define new_journal_request soap_new_set__ns1__Journal
#define soap_journal_call soap_call___ns1__Journal
#else //Linux
#define type_journal_request _tempuri__Journal
#define type_journal_response _tempuri__JournalResponse
#define new_journal_request soap_new_set__tempuri__Journal
#define soap_journal_call soap_call___tempuri__Journal
#endif

vector<int64_t> journal_list = {4707387510509010945 ,5067112530745229313};

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

void get_input(string *ServiceURL, string *journal_type) {

    // Getting all the input
    // ask for Service URL
    cout << "Please enter the serviceurl of the fiskaltrust.Service: ";
    getline(cin, *ServiceURL);

    // journal type
    cout << "Please choose the journal you want to request\
            \n1: \"AT DEP7\" \"0x4154 0000 0000 0001\"\
            \n2: \"FR Ticket\" \"0x4652 0000 0000 0001\"\
            \n: ";
    getline(cin, *journal_type);

    // trim the input strings
    trim(*ServiceURL);
    trim(*journal_type);

    // if ServiceURL end with '/' -> delete it
    if ((*ServiceURL).back() == '/') {
        (*ServiceURL).pop_back();
    }
}

int64_t *get_journal_type(string journal_type) {
    int index;
    if(!sscanf(journal_type.c_str(),"%d",&index)) {
        cerr << "ERROR input is no number" << endl;
        exit(-1);
    }

    if((size_t)index > journal_list.size()) {
        cerr << "ERROR invalid journal type" << endl;
        exit(-1);
    }
    index --;
    return &journal_list[index];
}

int main() {
    cout << "This example sends a journal request to the fiskaltrust.Service via SOAP\n";

    int64_t from = 0, to = 0;

    string ServiceURL, journal_type;

    get_input(&ServiceURL, &journal_type);

    //init echo class
    struct soap *ft = soap_new();

    class type_journal_request *journal_request = new_journal_request(ft, get_journal_type(journal_type), &from, &to);

    class type_journal_response journal_response;

    //make call
    cout << "making call... ";

    if(soap_journal_call(ft, ServiceURL.c_str(), nullptr, journal_request, journal_response) == SOAP_OK) {
        cout << "OK" << endl;
        cout << "Response: " << endl << journal_response.JournalResult.__ptr << endl;
    } else {
        cout << "done" << endl;
        soap_print_fault(ft, stderr);
    }

    soap_destroy(ft); // dealloc serialization data
    soap_end(ft);     // dealloc temp data
    soap_free(ft);    // dealloc 'soap' engine context
    return 0;
}
