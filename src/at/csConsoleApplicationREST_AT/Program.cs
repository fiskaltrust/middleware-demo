using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using Newtonsoft.Json;
using fiskaltrust.ifPOS.v0;

namespace csConsoleApplicationREST_AT
{
    class Program
    {

        static int i = 0;
        private static string url = "https://signaturcloud-sandbox.fiskaltrust.at/";
        private static Guid cashboxid = Guid.Empty;
        private static string accesstoken = "";
        private static bool json = true;

        static void Main(string[] args)
        {

            ServicePointManager.DefaultConnectionLimit = 65535;

            Console.Write("fiskaltrust-service-url (https://signaturcloud-sandbox.fiskaltrust.at/):");
            url = Console.ReadLine();
            if(url.Length==0)
            {
                url = "https://signaturcloud-sandbox.fiskaltrust.at/";
            }
            if(!url.EndsWith(url))
            {
                url += "/";
            }

            Console.Write("cashboxid:");
            string _cashboxid = Console.ReadLine();

            if (!Guid.TryParse(_cashboxid, out cashboxid))
            {
                throw new ArgumentException("cashboxid is not a guid!");
            }

            Console.Write("accesstoken:");
            accesstoken = Console.ReadLine();

            Console.Write("json (Y/n):");
            string _json = Console.ReadLine();
            if (_json.Length == 0)
                json = true;
            else if (_json.ToLower().StartsWith("n"))
                json = false;
            else
                json = true;


            if(json)
                echoJson(url, cashboxid, accesstoken,null);
            else
                echoXml(url, cashboxid, accesstoken);


            while (true)
            {
                Menu();
            }

        }

        static void Menu()
        {
            Console.WriteLine("1: Barumsatz (0x4154000000000001)");
            Console.WriteLine("2: Null-Beleg (0x4154000000000002)");
            Console.WriteLine("3: Inbetriebnahme-Beleg (0x4154000000000003)");
            Console.WriteLine("4: Außerbetriebnahme-Beleg (0x4154000000000004)");
            Console.WriteLine("5: Monats-Beleg (0x4154000000000005)");
            Console.WriteLine("6: Jahres-Beleg (0x4154000000000006)");

            Console.WriteLine("9: RKSV-DEP export");
            Console.WriteLine("10: Anzahl der zu sendenden Barumsatzbelege (max 999)");

            Console.WriteLine("exit: Program beenden");

            string input = Console.ReadLine();

            Command(input);
        }

        static void Command(string input)
        {
            if (input.ToLower().StartsWith("exit"))
            {
                var stream =  journalJson(0x4154000000000001, 0, DateTime.UtcNow.Ticks,url,cashboxid,accesstoken);
                var sr = new System.IO.StreamReader(stream);

                Console.WriteLine("{0:G} ========== RKSV-DEP ==========", DateTime.Now);

                Console.WriteLine(sr.ReadToEnd());

                Environment.Exit(0);
            }


            Random r = new Random((int)DateTime.Now.Ticks);

            int inputInt;
            if (!int.TryParse(input, out inputInt))
            {
                Console.WriteLine($"\"{input}\" nicht erkannt.");
                return;
            }

            if (inputInt == 1)
            {
                var req = UseCase17Request(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashboxid.ToString());
                Console.WriteLine("{0:G} Barumsatz request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));

                if(json)
                {
                    var resp = signJson(req,url,cashboxid,accesstoken);
                    Response(resp);
                }
                else
                {
                    var resp = signXml(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
            }
            else if (inputInt == 2)
            {
                var req = ZeroReceiptRequest(++i, cashboxid.ToString(), 0x4154000000000002);
                Console.WriteLine("{0:G} Null-Beleg request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                if (json)
                {
                    var resp = signJson(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
                else
                {
                    var resp = signXml(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
            }
            else if (inputInt == 3)
            {
                var req = ZeroReceiptRequest(++i, cashboxid.ToString(), 0x4154000000000003);
                Console.WriteLine("{0:G} Inbetriebnahme-Beleg request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                if (json)
                {
                    var resp = signJson(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
                else
                {
                    var resp = signXml(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
            }
            else if (inputInt == 4)
            {
                var req = ZeroReceiptRequest(++i, cashboxid.ToString(), 0x4154000000000004);
                Console.WriteLine("{0:G} Außerbetriebnahme-Beleg request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                if (json)
                {
                    var resp = signJson(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
                else
                {
                    var resp = signXml(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
            }
            else if (inputInt == 5)
            {
                var req = ZeroReceiptRequest(++i, cashboxid.ToString(), 0x4154000000000005);
                Console.WriteLine("{0:G} Monats-Beleg request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                if (json)
                {
                    var resp = signJson(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
                else
                {
                    var resp = signXml(req, url, cashboxid, accesstoken);
                    Response(resp);
                }

            }
            else if (inputInt == 6)
            {
                var req = ZeroReceiptRequest(++i, cashboxid.ToString(), 0x4154000000000006);
                Console.WriteLine("{0:G} Jahres-Beleg request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                if (json)
                {
                    var resp = signJson(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
                else
                {
                    var resp = signXml(req, url, cashboxid, accesstoken);
                    Response(resp);
                }
            }
            else if(inputInt==9)
            {

                string filename = $"c:\\temp\\{cashboxid}_{DateTime.UtcNow.Ticks}.json";

                Console.Write("{0:G} RKSV-DEP export ({1})", DateTime.Now, filename);

                string inputFilename = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(inputFilename))
                {
                    filename = inputFilename;
                }

                using (var file=System.IO.File.Open(filename, System.IO.FileMode.Create))
                {
                    using (var journal= journalJson(0x4154000000000001, 0,0 /*(new DateTime(2999,12,31)).Ticks*/, url, cashboxid, accesstoken))
                    {
                        journal.CopyTo(file);
                    }

                    Console.WriteLine("{0:G} RKSV-DEP exportiert nach {1} ({2:0.00}Mb)", DateTime.Now, filename,((decimal)file.Length)/1024m/1024m);
                }

                
            }
            else if (inputInt >= 10 && inputInt < 1000)
            {
                long max = long.MinValue;
                long min = long.MaxValue;
                long sum = 0;
                int n = 0;
                while (n++ < inputInt)
                {
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    var req = UseCase17Request(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashboxid.ToString());
                    ReceiptResponse resp;
                    if (json)
                    {
                        resp = signJson(req, url, cashboxid, accesstoken);
                        Response(resp);
                    }
                    else
                    {
                        resp = signXml(req, url, cashboxid, accesstoken);
                        Response(resp);
                    }
                    sw.Stop();
                    sum += sw.ElapsedMilliseconds;
                    if (sw.ElapsedMilliseconds > max) max = sw.ElapsedMilliseconds;
                    if (sw.ElapsedMilliseconds < min) min = sw.ElapsedMilliseconds;
                    Response(resp);
                }

                Console.WriteLine("Performance in ms => max: {0}, min: {1}, avg: {2}", max, min, sum / (decimal)inputInt);

            }
            else
            {
                Console.WriteLine($"\"{input}\" nicht erkannt.");
            }

            Menu();
        }


        internal static ReceiptRequest UseCase17Request(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = 0x4154000000000000,
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4154000000000000,
                         ProductNumber="1",
                         Description="Artikel 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4154000000000000,
                        ProductNumber="2",
                        Description="Artikel 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4154000000000000,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Bar"
                    }
                }
            };

            return reqdata;
        }

        internal static void Response(ReceiptResponse data)
        {
            if (data != null)
            {
                Console.WriteLine("{0:G} ========== response: {1}", DateTime.Now, JsonConvert.SerializeObject(data));
                Console.WriteLine("========== n: {0} CashBoxIdentificateion:{1} ReceiptIdentification:{2} ==========", data.cbReceiptReference, data.ftCashBoxIdentification, data.ftReceiptIdentification);
                foreach (var item in data.ftSignatures)
                {
                    if (item.ftSignatureFormat == 0x03)
                    {
                        fiskaltrust.ifPOS.TwoDCode.QR_TextChars(item.Data, 64, true);
                    }
                    else if (item.ftSignatureFormat == 0x08)
                    {
                        fiskaltrust.ifPOS.TwoDCode.AZTEC_TextChars(item.Data, 80, true);
                    }

                    Console.WriteLine("{0}:{1}", item.Caption, item.Data);

                    if (item.ftSignatureType == 0x4154000000000001)
                    {
                        //Console.WriteLine(fiskaltrust.ifPOS.Utilities.AT_RKSV_Signature_ToBase32(item.Data));
                        //Console.WriteLine(fiskaltrust.ifPOS.Utilities.AT_RKSV_Signature_ToLink(item.Data));
                    }
                }
            }
            else
            {
                Console.WriteLine("null-result!!!");
            }
        }

        internal static ReceiptRequest ZeroReceiptRequest(int n, string cashBoxId, long ftReceiptCase)
        {
            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = ftReceiptCase,
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,
                cbChargeItems = new ChargeItem[] { },
                cbPayItems = new PayItem[] { }
            };

            return reqdata;
        }



        static void echoJson(string url, Guid cashboxid = default(Guid), string accesstoken = null, string message="Hello World")
        {
            if (!url.EndsWith("/"))
                url += "/";

            var webreq = (HttpWebRequest)HttpWebRequest.Create(url + "json/echo");
            webreq.Method = "POST";
            webreq.ContentType = "application/json;charset=utf-8";

            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);

            byte[] reqecho = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            webreq.ContentLength = reqecho.Length;
            using (var reqStream = webreq.GetRequestStream())
            {
                reqStream.Write(reqecho, 0, reqecho.Length);
            }

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                using (var respReader = new System.IO.StreamReader(webresp.GetResponseStream(), Encoding.UTF8))
                {
                    var json = respReader.ReadToEnd();
                    var respecho = JsonConvert.DeserializeObject<string>(json);
                    Console.WriteLine("{0:G} Echo {1}", DateTime.Now, respecho);
                }
            }
            else
            {
                Console.WriteLine("{0:G} {1} {2}", DateTime.Now, webresp.StatusCode, webresp.StatusDescription);
            }
        }

        static void echoXml(string url, Guid cashboxid = default(Guid), string accesstoken = "00000000")
        {
            if (!url.EndsWith("/"))
                url += "/";

            string reqdata = "Hello World!";

            var ms = new System.IO.MemoryStream();
            var serializer = new DataContractSerializer(typeof(string));

            serializer.WriteObject(ms, reqdata);
            Console.WriteLine("{0:G} Request {1}", DateTime.Now, Encoding.UTF8.GetString(ms.ToArray()));

            var webreq = (HttpWebRequest)HttpWebRequest.Create(url + "xml/echo");
            webreq.Method = "POST";
            webreq.ContentType = "application/xml;charset=utf-8";

            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);


            webreq.ContentLength = ms.Length;
            using (var reqStream = webreq.GetRequestStream())
            {
                reqStream.Write(ms.ToArray(), 0, (int)ms.Length);
            }

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                ms = new System.IO.MemoryStream();
                webresp.GetResponseStream().CopyTo(ms);

                Console.WriteLine("{0:G} Echo {1}", DateTime.Now, Encoding.UTF8.GetString(ms.ToArray()));

                ms.Position = 0;
                string resp = (string)serializer.ReadObject(ms);
            }
            else
            {
                Console.WriteLine("{0:G} {1} {2}", DateTime.Now, webresp.StatusCode, webresp.StatusDescription);
            }
        }

        static ReceiptResponse signXml(ReceiptRequest request, string url, Guid cashboxid = default(Guid), string accesstoken = "00000000")
        {
            if (!url.EndsWith("/"))
                url += "/";

            var ms = new System.IO.MemoryStream();
            var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(fiskaltrust.ifPOS.v0.ReceiptRequest));

            serializer.WriteObject(ms, request);

            var webreq = (HttpWebRequest)HttpWebRequest.Create(url + "xml/sign");
            webreq.Method = "POST";
            webreq.ContentType = "application/xml;charset=utf-8";

            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);

            webreq.ContentLength = ms.Length;
            using (var reqStream = webreq.GetRequestStream())
            {
                reqStream.Write(ms.ToArray(), 0, (int)ms.Length);
            }

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(fiskaltrust.ifPOS.v0.ReceiptResponse));
                ms = new System.IO.MemoryStream();
                webresp.GetResponseStream().CopyTo(ms);

                ms.Position = 0;
                fiskaltrust.ifPOS.v0.ReceiptResponse resp = (fiskaltrust.ifPOS.v0.ReceiptResponse)serializer.ReadObject(ms);
                return resp;
            }
            else
            {
                return null;
            }

        }

        static ReceiptResponse  signJson(fiskaltrust.ifPOS.v0.ReceiptRequest request, string url, Guid cashboxid = default(Guid), string accesstoken = "00000000")
        {
            if (!url.EndsWith("/"))
                url += "/";

            var jsonSettings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            var reqjson = JsonConvert.SerializeObject(request, jsonSettings);

            var webreq = (HttpWebRequest)HttpWebRequest.Create(url + "json/sign");
            webreq.Method = "POST";
            webreq.ContentType = "application/json;charset=utf-8";

            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);


            byte[] reqbytes = Encoding.UTF8.GetBytes(reqjson);
            webreq.ContentLength = reqbytes.Length;
            using (var reqStream = webreq.GetRequestStream())
            {
                reqStream.Write(reqbytes, 0, reqbytes.Length);
            }

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                using (var respReader = new System.IO.StreamReader(webresp.GetResponseStream(), Encoding.UTF8))
                {
                    string respstring = respReader.ReadToEnd();
                    var respdata = JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(respstring, jsonSettings);
                    return respdata;
                }
            }
            else
            {
                return null;
            }
        }

        static System.IO.Stream journalJson(long ftJournalType,long from,long to,  string url, Guid cashboxid = default(Guid), string accesstoken = "00000000")
        {
            if (!url.EndsWith("/"))
                url += "/";

            var webreq = (HttpWebRequest)HttpWebRequest.Create(String.Format("{0}json/journal?type={1}&from={2}&to={3}", url, ftJournalType,from,to));
            webreq.Method = "POST";
            webreq.ContentType = "application/json;charset=utf-8";
            webreq.ContentLength = 0;

            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                using (var respStream = webresp.GetResponseStream())
                {
                    var ms = new System.IO.MemoryStream();
                    respStream.CopyTo(ms);
                    ms.Position = 0;
                    return ms;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
