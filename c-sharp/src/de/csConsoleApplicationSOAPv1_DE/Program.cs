using fiskaltrust.ifPOS.v1;
using Newtonsoft.Json;
using System;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;

namespace csConsoleApplicationSOAPv1_DE
{
    class Program
    {
        static int i = 0;
        static string url;
        static string cashBoxId;
        static string lastTransactionId = string.Empty;

        static IPOS proxy = null;
        private const int MAX_GENERATED_RECEIPT_COUNT = 100000;

        static void Main(string[] args)
        {

            ServicePointManager.DefaultConnectionLimit = 65535;

            var options = ProgramOptions.GetOptionsFromCommandLine(args);

            url = options.url;
            cashBoxId = options.cashboxid.ToString();

            System.ServiceModel.Channels.Binding binding = null;

            if (url.StartsWith("http://"))
            {
                var b = new BasicHttpBinding(BasicHttpSecurityMode.None);
                b.MaxReceivedMessageSize = 16 * 1024 * 1024;

                binding = b;
            }
            else if (url.StartsWith("https://"))
            {
                var b = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                b.MaxReceivedMessageSize = 16 * 1024 * 1024;

                binding = b;
            }
            else if (url.StartsWith("net.pipe://"))
            {
                var b = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                b.MaxReceivedMessageSize = 16 * 1024 * 1024;

                binding = b;
            }
            else if (url.StartsWith("net.tcp://"))
            {
                var b = new NetTcpBinding(SecurityMode.None);
                b.MaxReceivedMessageSize = 16 * 1024 * 1024;

                binding = b;

            }

            var endpoint = new EndpointAddress(url);

            var factory = new ChannelFactory<IPOS>(binding, endpoint);

            proxy = factory.CreateChannel();


            // use echo for communication test
            var message = proxy.EchoAsync(new EchoRequest() { Message = "message" }).Result;
            if (message.Message != "message") throw new Exception("echo failed");

            while (true)
            {
                Menu();
            }
        }

        static void Menu()
        {
            if (!string.IsNullOrWhiteSpace(lastTransactionId))
            {
                Console.WriteLine($"");
                Console.WriteLine($"------------------------------------------------");
                Console.WriteLine($"Last TransactionId: {lastTransactionId}");
                Console.WriteLine($"------------------------------------------------");
                Console.WriteLine($"");
            }

            Console.WriteLine("1: Begin Transaction (0x4445000000000008)");
            Console.WriteLine("2: Update Transaction (0x4445000000000009)");
            Console.WriteLine("3: POS Receipt with explicit flow (0x4445000000000001)");
            Console.WriteLine("4: Start Receipt (0x4445000000000003)");
            Console.WriteLine("5: Stop Receipt (0x4445000000000004)");
            Console.WriteLine("6: Daily Closing Receipt (0x4445000000000007)");
            Console.WriteLine("7: POS Receipt with implicit flow (0x4445000100000001)");

            Console.WriteLine("9: Status export");

            Console.WriteLine("exit: Exit Program");

            string input = Console.ReadLine();

            Command(input);
        }

        static void Command(string input)
        {
            if (input.ToLower().StartsWith("exit"))
            {
                var journal = proxy.JournalAsync(new JournalRequest() { ftJournalType = 0x4445000000000000, From = 0, To = DateTime.UtcNow.Ticks }).Result;

                Console.WriteLine("{0:G} ========== Status export ==========", DateTime.Now);

                Console.WriteLine(journal.Journal);

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
                var req = ZeroReceiptRequest(++i, cashBoxId, 0x4445000000000008);
                Console.WriteLine("{0:G} Begin Transaction request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                if (resp != null && resp.ftReceiptIdentification.Contains("#ST"))
                {
                    lastTransactionId = req.cbReceiptReference;
                }

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));
            }
            else if (inputInt == 2)
            {
                var req = UseCaseRequest(lastTransactionId, 0x4445000000000009, (decimal)((new Random()).NextDouble() * 10.0), (decimal)((new Random()).NextDouble() * 10.0), cashBoxId);
                Console.WriteLine("{0:G} Update Transaction request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));
            }
            else if (inputInt == 3)
            {
                var req = UseCaseRequest(lastTransactionId, 0x4445000000000001, (decimal)((new Random()).NextDouble() * 10.0), (decimal)((new Random()).NextDouble() * 10.0), cashBoxId);
                Console.WriteLine("{0:G} End Transaction request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));
            }
            else if (inputInt == 4)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, 0x4445000000000003);
                Console.WriteLine("{0:G} Start request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));
            }
            else if (inputInt == 5)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, 0x4445000000000004);
                Console.WriteLine("{0:G} Stop request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));

            }
            else if (inputInt == 6)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, 0x4445000000000007);
                Console.WriteLine("{0:G} Daily Closing request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));
            }
            else if (inputInt == 7)
            {
                var req = UseCaseRequest(lastTransactionId, 0x4445000100000001, (decimal)((new Random()).NextDouble() * 10.0), (decimal)((new Random()).NextDouble() * 10.0), cashBoxId);
                Console.WriteLine("{0:G} POS receipt with Implicit Transaction: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.SignAsync(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v1.ReceiptRequest>(JsonConvert.SerializeObject(req))).Result;

                Response(JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptResponse>(JsonConvert.SerializeObject(resp)));
            }
            else if (inputInt == 9)
            {

                string filename = $"c:\\temp\\{cashBoxId}_{DateTime.UtcNow.Ticks}.json";

                Console.Write("{0:G} Status export ({1})", DateTime.Now, filename);

                string inputFilename = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputFilename))
                {
                    filename = inputFilename;
                }

                using (var file = System.IO.File.Open(filename, System.IO.FileMode.Create))
                {
                    var journal = proxy.JournalAsync(new JournalRequest() { ftJournalType = 0x4445000000000000, From = 0, To = 0 }).Result;
                    System.IO.File.WriteAllText(filename, journal.Journal);

                    Console.WriteLine("{0:G} Status exportiert nach {1} ({2:0.00}Mb)", DateTime.Now, filename, ((decimal)file.Length) / 1024m / 1024m);
                }
            }
            else
            {
                Console.WriteLine($"\"{input}\" nicht erkannt.");
            }

            Menu();
        }


        internal static fiskaltrust.ifPOS.v0.ReceiptRequest UseCaseRequest(string receiptReference, long receiptCase, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new fiskaltrust.ifPOS.v0.ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = receiptCase,
                cbReceiptReference = receiptReference,
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new fiskaltrust.ifPOS.v0.ChargeItem[]  {
                    new fiskaltrust.ifPOS.v0.ChargeItem()
                    {
                        ftChargeItemCase=0x4445000000000001,
                         ProductNumber="1",
                         Description="Artikel 1",
                         Quantity=1.0m,
                         VATRate=19.0m,
                         Amount=amount1
                    },
                    new fiskaltrust.ifPOS.v0.ChargeItem()
                    {
                        ftChargeItemCase=0x4445000000000001,
                        ProductNumber="2",
                        Description="Artikel 2",
                        Quantity=1.0m,
                        VATRate=19.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new fiskaltrust.ifPOS.v0.PayItem[]                {
                    new fiskaltrust.ifPOS.v0.PayItem()
                    {
                        ftPayItemCase=0x4445000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Bar"
                    }
                }
            };

            return reqdata;
        }

        internal static void Response(fiskaltrust.ifPOS.v0.ReceiptResponse data)
        {
            if (data != null)
            {
                Console.WriteLine("{0:G} ========== response: {1}", DateTime.Now, JsonConvert.SerializeObject(data));
                Console.WriteLine("========== n: {0} CashBoxIdentificateion:{1} ReceiptIdentification:{2} ==========", data.cbReceiptReference, data.ftCashBoxIdentification, data.ftReceiptIdentification);
                foreach (var item in data.ftSignatures)
                {
                    if (item.ftSignatureFormat == 0x03)
                    {
                        if (item.Data.Length <= 300)
                        {
                            fiskaltrust.ifPOS.TwoDCode.QR_TextChars(item.Data, 80, true);
                        }
                        else if (item.Data.Length > 300 && item.Data.Length <= 800)
                        {
                            fiskaltrust.ifPOS.TwoDCode.QR_TextChars(item.Data, 100, true);
                        }
                        else if (item.Data.Length > 800 && item.Data.Length <= 1000)
                        {
                            fiskaltrust.ifPOS.TwoDCode.QR_TextChars(item.Data, 125, true);
                        }
                        else if (item.Data.Length > 1000 && item.Data.Length <= 1200)
                        {
                            fiskaltrust.ifPOS.TwoDCode.QR_TextChars(item.Data, 150, true);
                        }
                        else if (item.Data.Length > 1200)
                        {
                            fiskaltrust.ifPOS.TwoDCode.QR_TextChars(item.Data, 200, true);
                        }
                    }
                    else if (item.ftSignatureFormat == 0x08)
                    {
                        fiskaltrust.ifPOS.TwoDCode.AZTEC_TextChars(item.Data, 80, true);
                    }

                    Console.WriteLine("{0}:{1}", item.Caption, item.Data);

                    if (item.ftSignatureType == 0x4154000000000001 && item.ftSignatureFormat == 3)
                    {
                        Console.WriteLine(fiskaltrust.ifPOS.Utilities.AT_RKSV_Signature_ToBase32(item.Data));
                    }
                }
            }
            else
            {
                Console.WriteLine("null-result!!!");
            }
        }

        internal static fiskaltrust.ifPOS.v0.ReceiptRequest ZeroReceiptRequest(int n, string cashBoxId, long ftReceiptCase)
        {
            var reqdata = new fiskaltrust.ifPOS.v0.ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = ftReceiptCase,
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,
                cbChargeItems = new fiskaltrust.ifPOS.v0.ChargeItem[] { },
                cbPayItems = new fiskaltrust.ifPOS.v0.PayItem[] { }
            };

            return reqdata;
        }
    }
}
