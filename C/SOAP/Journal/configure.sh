#!/bin/bash
if [ "$1" = "-h" ]; then
    echo "help"
    echo "   -WSDL_PATH [PATH/to/file.WSDL]"
    exit 0
fi

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
    if [ ${!i} = "-WSDL_PATH" ]; then
        #echo "WSDL"
        WSDL_PATH=${!tmp}
    fi
done

if [ -z $WSDL_PATH ]; then 
    echo "location to WSDL file: "
    read WSDL_PATH
fi
if [ ! -f "$WSDL_PATH" ]; then
    echo "wsdl file not found at: $WSDL_PATH"
    exit -1
fi

#write Makefile
IN=`cat Makefile.in`

rm Makefile
echo "PATH_to_WSDL = $WSDL_PATH" >> Makefile
echo -e "" >> Makefile #newline
echo "$IN" >> Makefile

echo "Configuration sucessfull"