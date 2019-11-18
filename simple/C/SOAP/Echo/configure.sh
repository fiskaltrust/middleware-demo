#!/bin/bash
if [ "$1" = "-h" ]; then
    echo "help"
    #......

    exit 0
fi
#default
gSOAP = "n"

#get parameter if available
for i in {1..1..2}
do
    if [ -z ${!i} ]; then 
        break
    fi
    tmp=$(( i+1 ))
    if [ -z ${!tmp} ]; then 
        echo "invalid syntax"
        exit -1
    fi
    if [ ${!i} = "-gSOAP" ]; then
        #echo "gSoap"
        gSOAP=${!tmp}
    fi
    if [ ${!i} = "-WSDL_PATH" ]; then
        #echo "WSDL"
        WSDL_PATH=${!tmp}
    fi
done

if [ -z $gSOAP ]; then 
    echo "Should gsoap be installed (y/n) (default: 'n'): "
    read gSOAP
fi

if [ -z $WSDL_PATH ]; then 
    echo "location to WSDL file: "
    read WSDL_PATH
fi
if [ ! -f "$WSDL_PATH" ]; then
    echo "wsdl file not found at $WSDL_PATH"
    exit -1
fi

#dowanload gsaop
if [ $gSOAP = "y" ]; then
    #Ubuntu debian ?
    sudo apt install gsoap -y
fi

#write Makefile
IN=`cat Makefile.in`

rm Makefile
echo "PATH_to_WSDL = $WSDL_PATH" >> Makefile
echo -e "" >> Makefile #newline
echo "$IN" >> Makefile

echo "Configuration sucessfull"