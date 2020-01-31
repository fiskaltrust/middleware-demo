#!/bin/bash
if [ "$1" = "-h" ]; then
    echo "help"
    echo "   -PATH-httplib [PATH/to/cpp-httplib]"
    echo "   -PATH-json [PATH/to/json]"
    exit 0
fi

#get parameter if available
for i in {1..3..2}
do
    if [ -z ${!i} ]; then 
        break
    fi
    tmp=$(( i+1 ))
    if [ -z ${!tmp} ]; then 
        echo "invalid syntax"
        exit -1
    fi
    if [ ${!i} = "-PATH-httplib" ]; then
        PARH_httplib=${!tmp}
    fi
    if [ ${!i} = "-PATH-json" ]; then
        PARH_json=${!tmp}
    fi
done

if [ -z $PARH_httplib ]; then 
    echo "location to cpp-httplib (default:./lib/cpp-httplib): "
    read PARH_httplib
    if [ -z $PARH_httplib ]; then 
        PARH_httplib="./lib/cpp-httplib"
    fi
fi

#cut '/' if at end of path
if [ "${PARH_httplib: -1}" == "/" ]; then
    PARH_httplib="${PARH_httplib::-1}"
fi

#check existance
if [ ! -d "$PARH_httplib" ]; then
    echo "cpp-httplib not found at: $PARH_httplib"
    exit -1
fi

if [ -z $PARH_json ]; then 
    echo "location to json lib (default:./lib/json): "
    read PARH_json
    if [ -z $PARH_json ]; then 
        PARH_json="./lib/json"
    fi
fi

#cut '/' if at end of path
if [ "${PARH_json: -1}" == "/" ]; then
    PARH_json="${PARH_json::-1}"
fi

#check existance
if [ ! -d "$PARH_json" ]; then
    echo "json lib not found at: $PARH_json"
    exit -1
fi

#write Makefile
IN=`cat Makefile.in`

rm Makefile
echo "PATH_cpp_httplib = \"$PARH_httplib\"" >> Makefile
echo "PATH_json = \"$PARH_json\"" >> Makefile
echo -e "" >> Makefile #newline
echo "$IN" >> Makefile

#create cpp-httplib .cc and .h
cd "$PARH_httplib" ; python3 split.py

echo "Configuration sucessfull"