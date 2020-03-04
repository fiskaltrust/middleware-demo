function test() {
    console.log("test start");

    var url = document.querySelector("#serviceurl").value;
    url += "/json/sign";
    console.log(url);

    var reqdata = document.querySelector("#reqdata").value.replace(/\:\s*([0-9]{15,})\s*([,}\]])/g, ':"$1"$2');
    console.log(reqdata);
    var obj = JSON.parse(reqdata);
    console.log(obj);

    fetch(url, {
        method: 'POST',
        headers: {
            "cashboxid": document.querySelector("#cashboxid").value,
            "accesstoken": document.querySelector("#accesstoken").value,
            "Content-Type": "application/json"
        },
        body: reqdata
    })
    .then(success)
    .catch(error);

    console.log("test end");
}

async function success(data) {
    text = (await data.text()).replace(/\:\s*([0-9]{15,})\s*([,}\]])/g, ':"$1"$2');
    console.log(data.statusText);
    console.log(JSON.parse(text));
    document.querySelector("#respdata").value = text;
}

function error(data) {
    console.log(data);
    document.querySelector("#respdata").value = data;
}
