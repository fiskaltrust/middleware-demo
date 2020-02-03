#!/bin/bash
if [ "$1" = "-h" ]; then
    echo "help"
    echo "   -PATH_WSDL [PATH/to/file.WSDL]"
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
    if [ ${!i} = "-PATH_WSDL" ]; then
        #echo "WSDL"
        PATH_WSDL=${!tmp}
    fi
done

#WSDL
if [ -z $PATH_WSDL ]; then 
    echo "location to WSDL file: "
    read PATH_WSDL
fi
if [ ! -f "$PATH_WSDL" ]; then
    echo "wsdl file not found at: $PATH_WSDL"
    exit -1
fi

#write Makefile
IN=`cat Makefile.in`

rm Makefile
echo "PATH_to_WSDL = \"$PATH_WSDL\"" >> Makefile
echo -e "" >> Makefile #newline
echo "$IN" >> Makefile

echo "Configuration sucessfull"