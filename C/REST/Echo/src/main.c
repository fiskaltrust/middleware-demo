#include <stdio.h> /* printf, sprintf */
#include <stdlib.h> /* exit, atoi, malloc, free */
#include <string.h>
#include <curl/curl.h>
#include <inttypes.h> //int64_t

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

void add_Qotes(char *body) {
    body[0] = 34; //ASCII for >"<
    for(int i = 0; ;i++) {
        if(body[i] == 0) {
            body[i] = 34; //ASCII for >"<
            body[i+1] = 0;
            break;
        }
    }
}

void get_input(char *ServiceURL, char *cashboxid, char *accesstoken, char *body) {
    
    char buffer[STRING_LENGTH];
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

    //get echo body
    printf("Please enter the body to send in the echo request(no more then %d caracters): ",(STRING_LENGTH-3));
    fgets(buffer,STRING_LENGTH-3,stdin);

    //trim the input strings
    trim(ServiceURL, NULL);
    trim(cashboxid, NULL);
    trim(accesstoken, NULL);
    trim(buffer, NULL);

    // add quotes to beginning and end of body
    sprintf(body, "\"%s\"",buffer);

    //if ServiceURL end with '/' -> delete it
    if(ServiceURL[strlen(ServiceURL) -1] == '/') {ServiceURL[strlen(ServiceURL) -1] = 0;}

    //add_Qotes(body);
}

void send_request(char *ServiceURL, char *cashboxid, char *accesstoken, char *body, struct response *s, int64_t *response_code) {
    
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
    strcat(requestURL, "/json/echo"); //add endpoint

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

        // set response
        //set callback function
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_callback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, s);

        // Perform the request, res will get the return code
        printf("performing request ...");
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
    printf("This example sends an echo request to the fiskaltrust.Service via REST\n");

    char ServiceURL[STRING_LENGTH];
    char cashboxid[STRING_LENGTH];
    char accesstoken[STRING_LENGTH];
    char body[STRING_LENGTH];

    get_input(ServiceURL, cashboxid, accesstoken, body);

    //init response struct
    struct response s;
    init_string(&s);

    int64_t response_code;

    send_request(ServiceURL, cashboxid, accesstoken, body, &s, &response_code);

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
        free(s.ptr);
    }
    
    return 0;
}
