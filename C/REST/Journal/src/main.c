#include <stdio.h> /* printf, sprintf */
#include <stdlib.h> /* exit, atoi, malloc, free */
#include <string.h>
#include <curl/curl.h>
#include <inttypes.h>
#include <time.h>
#include <cJSON/cJSON.h>

#define STRING_LENGTH 256

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

void get_input(char *ServiceURL, char *cashboxid, char *accesstoken, char *countryCode) {
    
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

    //get country Code
    printf("Please enter the countrycode of the fiskaltrust.Queue (e.g. \"AT,DE,FR\"): ");
    fgets(countryCode,STRING_LENGTH-1,stdin);

    //trim the input strings
    trim(ServiceURL, NULL);
    trim(cashboxid, NULL);
    trim(accesstoken, NULL);
    trim(countryCode, NULL);

    string_to_UPPERcase(countryCode);

    //check countyCode length
    if(strlen(countryCode) != 2) {
        printf("The countrycode must have length two.\n");
        exit(EXIT_FAILURE);
    }
}

uint64_t build_journal_type(char *countryCode) {
    uint64_t receipt_case = 0;

    //add county Code
    receipt_case |= ((uint64_t)countryCode[0] << (4 * 14));
    receipt_case |= ((uint64_t)countryCode[1] << (4 * 12));

    //journal request
    receipt_case |= 1;

    return receipt_case;
}

void send_request(char *ServiceURL, char *cashboxid, char *accesstoken, struct response *s, int *response_code, int64_t journal_type) {
    
    CURL *curl = NULL;
    CURLcode res;
    struct curl_slist *header = NULL;
    
    #ifdef _WIN32
        //init socket for Windows
        curl_global_init(CURL_GLOBAL_ALL);
    #endif

    //init curl
    curl = curl_easy_init();
    
    char cer_path[] = {".\\"};
    char cer_name[] = {"curl-ca-bundle.crt"};
    char requestURL[STRING_LENGTH], buffer[STRING_LENGTH];
    strcpy(requestURL, ServiceURL);
    strcat(requestURL, "/json/journal"); //add endpoint

    //quert parameter
    sprintf(buffer,"?type=%"PRId64,journal_type);
    strcat(requestURL, buffer);
    strcat(requestURL, "&from=0");
    strcat(requestURL, "&to=0");
    printf("requestURL: %s\n",requestURL);

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

        // set empty POS type and empty body
        curl_easy_setopt(curl, CURLOPT_POSTFIELDS, ""); 

        // set our Service Url
        curl_easy_setopt(curl, CURLOPT_URL, requestURL);

        // tell libcurl to follow redirection
        curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);

        // set verify certificate
        //curl_easy_setopt(curl, CURLOPT_CAINFO, cer_name); //add curl certificate
        //curl_easy_setopt(curl, CURLOPT_CAPATH, cer_path); //path to certificate

         
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
    printf("This example sends a journal request to the fiskaltrust.Service via REST\n");
    
    char ServiceURL[STRING_LENGTH];
    char cashboxid[STRING_LENGTH];
    char accesstoken[STRING_LENGTH];
    char countryCode[STRING_LENGTH];

    get_input(ServiceURL, cashboxid, accesstoken, countryCode);
       

    //init response struct
    struct response s;
    init_string(&s);

    uint64_t journal_type = build_journal_type(countryCode);
    #ifdef DEBUG
        printf("URL: %s\n",ServiceURL);
    #endif

    int response_code;
    send_request(ServiceURL, cashboxid, accesstoken, &s, &response_code, journal_type);

    //print Response
    
    printf("Response Code: %ld\n",response_code);
    if(s.ptr[0] == 0) {
        printf("No Response\n");
    }
    else {
        printf("Body:\n%s\n", s.ptr);
         //parse JSON
        cJSON *response_obj = cJSON_Parse(s.ptr);
        printf("name of obj: %s\n",response_obj->child->child->child->next->string);
        cJSON_free(response_obj);
        response_obj = NULL;
        free(s.ptr);
        s.ptr = NULL;
    }

    return 0;
}