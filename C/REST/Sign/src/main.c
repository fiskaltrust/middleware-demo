#include <stdio.h> /* printf, sprintf */
#include <stdlib.h> /* exit, atoi, malloc, free */
#include <string.h>
#include <curl/curl.h>
#include <inttypes.h> //int64_t, uint64_t
#include <time.h>
#include <json-c/json.h>

#define STRING_LENGTH 256

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

struct response{
  char *ptr;
  size_t len;
};

void init_string(struct response *s) {
  s->len = 0;
  s->ptr = malloc(s->len+1);
  if (s->ptr == NULL) {
    fprintf(stderr, "malloc() failed\n");
    exit(EXIT_FAILURE);
  }
  s->ptr[0] = '\0';
}

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
        }
        else {
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

// https://curl.haxx.se/libcurl/c/CURLOPT_WRITEFUNCTION.html
size_t write_callback(char *ptr, size_t size, size_t nmemb, struct response *s) {
    size_t new_len = s->len + size*nmemb;
    s->ptr = realloc(s->ptr, new_len+1);
    if (s->ptr == NULL) {
        fprintf(stderr, "realloc() failed\n");
        exit(EXIT_FAILURE);
    }
    memcpy(s->ptr+s->len, ptr, size*nmemb);
    s->ptr[new_len] = '\0';
    s->len = new_len;

    return size*nmemb;
}

void string_to_UPPERcase(char *target) {
    for(int i = 0; target[i] != 0 ;i++) {
        if(target[i] <= 'z' && target[i] >= 'a') { target[i] &= (~(1<<5));} //the differenc from UPPER case to lower case is 32 so we unset the 5th bit 
    }
}

void get_input(char *ServiceURL, char *cashboxid, char *accesstoken, char *country, char *POSsystemID, int *receipt) {
    
    char temp[STRING_LENGTH] = {0};
    //Getting all the input
    //ask for Service URL
    printf("Please enter the serviceurl of the fiskaltrust.Service: ");
    fgets(ServiceURL,STRING_LENGTH-1,stdin);

    //get cashboxID
    printf("Please enter the cashboxid of the fiskaltrust.CashBox: ");
    fgets(cashboxid,STRING_LENGTH-1,stdin);

    //get accesstoken
    printf("Please enter the accesstoken of the fiskaltrust.CashBox: ");
    fgets(accesstoken,STRING_LENGTH-1,stdin);

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
    
    //trim the input strings
    trim(ServiceURL, NULL);
    trim(cashboxid, NULL);
    trim(accesstoken, NULL);
    trim(POSsystemID, NULL);
    trim(country, NULL);

    string_to_UPPERcase(country);

    //check countyCode length
    if(strlen(country) != 2) {
        printf("The country must have length two.\n");
        exit(EXIT_FAILURE);
    }

    //if ServiceURL end with '/' -> delete it
    if(ServiceURL[strlen(ServiceURL) -1] == '/') {ServiceURL[strlen(ServiceURL) -1] = 0;}
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

json_object *set_zero_body(char *cashboxid, char *POSsystemID, int64_t receipt_case) {
    char buffer[128];

    json_object *root = json_object_new_object();
    json_object_object_add(root,"ftCashBoxID",json_object_new_string(cashboxid));
    json_object_object_add(root,"ftPosSystemId",json_object_new_string(POSsystemID));
    json_object_object_add(root,"cbTerminalID",json_object_new_string("1"));
    json_object_object_add(root,"cbReceiptReference",json_object_new_string("1"));
    sprintf(buffer,"/Date(%lu)/",(unsigned long)time(NULL));
    json_object_object_add(root,"cbReceiptMoment",json_object_new_string(buffer));
    json_object_object_add(root,"cbChargeItems",json_object_new_array());
    json_object_object_add(root,"cbPayItems",json_object_new_array());
    json_object_object_add(root,"ftReceiptCase",json_object_new_int64(receipt_case));
    return root;
}

json_object *set_cash_body(char *cashboxid, char *POSsystemID, int64_t receipt_case, char *country) {
    char buffer[128];
    int country_index;
    if(strcmp(country, "AT") == 0) {country_index = 0;}
    else if(strcmp(country, "DE") == 0) {country_index = 1;}
    else if(strcmp(country, "FR") == 0) {country_index = 2;}
    
    //create cbChargeItems array
    //create ChargeItem
    json_object *item = json_object_new_object();
    json_object_object_add(item,"Quantity",json_object_new_double(10.0));
    json_object_object_add(item,"Description",json_object_new_string("Food"));
    json_object_object_add(item,"Amount",json_object_new_double(5.0));
    json_object_object_add(item,"VATRate",json_object_new_double(10.0));
    json_object_object_add(item,"ftChargeItemCase",json_object_new_int64(ChargeItemCase[country_index]));
    json_object_object_add(item,"ProductNumber",json_object_new_string("1"));

    //add item to array
    json_object *ChargeItems = json_object_new_array();
    json_object_array_add(ChargeItems,item); //the same function can be used to add more objects

    
    //create cbPayItems array
    //creat cbPayItem
    json_object *PayItem = json_object_new_object();
    json_object_object_add(PayItem,"Quantity",json_object_new_double(10.0));
    json_object_object_add(PayItem,"Description",json_object_new_string("Cash"));
    json_object_object_add(PayItem,"Amount",json_object_new_double(5.0));
    json_object_object_add(PayItem,"ftPayItemCase",json_object_new_int64(PayItemCase[country_index]));

    //add item to array
    json_object *PayItems = json_object_new_array();
    json_object_array_add(PayItems,PayItem); //the same function can be used to add mor objects


    //create root object
    json_object *root = json_object_new_object();
    json_object_object_add(root,"ftCashBoxID",json_object_new_string(cashboxid));
    json_object_object_add(root,"ftPosSystemId",json_object_new_string(POSsystemID));
    json_object_object_add(root,"cbTerminalID",json_object_new_string("1"));
    json_object_object_add(root,"cbReceiptReference",json_object_new_string("1"));
    sprintf(buffer,"/Date(%lu)/",(unsigned long)time(NULL));
    json_object_object_add(root,"cbReceiptMoment",json_object_new_string(buffer));
    json_object_object_add(root,"cbChargeItems",ChargeItems);
    json_object_object_add(root,"cbPayItems",PayItems);
    json_object_object_add(root,"ftReceiptCase",json_object_new_int64(receipt_case));

    return root;
}

void send_request(char *ServiceURL, char *cashboxid, char *accesstoken, const char *body, struct response *s, int64_t *response_code) {
    
    CURL *curl = NULL;
    CURLcode res;
    char buffer[STRING_LENGTH + 128];
    struct curl_slist *header = NULL;
    
    #ifdef _WIN32
        //init socket for Windows
        curl_global_init(CURL_GLOBAL_ALL);
    #endif

    //init curl
    curl = curl_easy_init();
    
    #ifdef _WIN32
        char cer_path[] = {".\\"};
        char cer_name[] = {"curl-ca-bundle.crt"};
    #endif
    char requestURL[STRING_LENGTH];
    strcpy(requestURL, ServiceURL);
    strcat(requestURL, "/json/sign"); //add endpoint

    if (curl)
    {
         
        // Add header content
        // Content-Type
        header = curl_slist_append(header, "Content-Type: application/json");

        // Cashbox id
        sprintf(buffer,"cashboxid: %s",cashboxid);
        header = curl_slist_append(header, buffer);

        // Cashbox id
        sprintf(buffer,"accesstoken: %s",accesstoken);
        header = curl_slist_append(header, buffer);

        // set our custom set of headers
        curl_easy_setopt(curl, CURLOPT_HTTPHEADER, header);

        // set our body
        curl_easy_setopt(curl, CURLOPT_POSTFIELDS, body);

        // set our Service Url
        curl_easy_setopt(curl, CURLOPT_URL, requestURL);

        // tell libcurl to follow redirection
        curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);

        #ifdef _WIN32
            // set verify certificate
            curl_easy_setopt(curl, CURLOPT_CAINFO, cer_name); //add curl certificate
            curl_easy_setopt(curl, CURLOPT_CAPATH, cer_path); //path to certificate
        #endif

         
        // get header with callback
        //curl_easy_setopt(curl, CURLOPT_HEADER, 1);

        // set response
        //set callback function
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_callback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, s);

        // Perform the request, res will get the return code
        printf("performing request...");
        res = curl_easy_perform(curl);
        
        // Check for errors
        if (res != CURLE_OK) {
            printf(" failed\n");
            fprintf(stderr, "curl_easy_perform() failed: %s\n",
                    curl_easy_strerror(res));
        }
        else {
            printf(" OK\n");
            curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, response_code);
        }
        // always cleanup
        curl_easy_cleanup(curl);
        curl_slist_free_all(header); //free header list
        
    }
    else {
        fprintf(stderr,"ERROR curl easy init failed!\n");
    }
}

int main()
{
    printf("This example sends a sign request to the fiskaltrust.Service via REST\n");
    
    char ServiceURL[STRING_LENGTH];
    char cashboxid[STRING_LENGTH];
    char accesstoken[STRING_LENGTH];
    char country[STRING_LENGTH];
    char POSsystemID[STRING_LENGTH];
    int receipt;

    get_input(ServiceURL, cashboxid, accesstoken, country, POSsystemID, &receipt);
    
    int64_t response_code;

    //init response struct
    struct response s;
    init_string(&s);

    int64_t receip_case = get_receipt_case(country, receipt);

    json_object *body;
    if(receipt == 3) { //cash signing
        body = set_cash_body(cashboxid, POSsystemID, receip_case, country);
    }
    else{
        body = set_zero_body(cashboxid, POSsystemID, receip_case);
    }
        
    
    send_request(ServiceURL, cashboxid, accesstoken, json_object_to_json_string(body), &s, &response_code);
    json_object_put(body);

    //print Response
    if(s.ptr[0] == 0) {
        printf("No Response\n");
    }
    else {
        #ifdef _WIN32
        printf("Response Code: %I64d\n",response_code);
        #else
        printf("Response Code: %ld\n",response_code);
        #endif
        printf("Body:\n%s\n", s.ptr);

        //parse response body to json
        json_object *target = NULL;
        json_object *response_json = json_tokener_parse(s.ptr);
        json_pointer_get(response_json,"/ftState",&target);
        if(target) {printf("ftState: %s\n",json_object_to_json_string(target));}
        free(s.ptr);
    }
    
    return 0;
}