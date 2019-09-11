using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

using fiskaltrust.ifPOS.v0;
using Newtonsoft.Json;
using System.Net;

namespace csConsoleApplicationSOAP_FR
{
    class Program
    {
        static int i = 0;
        static string url;
        static string cashBoxId;
        static bool isTraining = false;

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
            var message = proxy.Echo("message");
            if (message != "message") throw new Exception("echo failed");


            while (true)
            {
                Menu();
            }
        }

        static void Menu()
        {
            if (isTraining)
            {
                Console.WriteLine("TRAINING MODE ACTIVE!");
            }

            Console.WriteLine($"1: Ticket (0x{GetReceiptCase(0x4652000000000001):X})");
            Console.WriteLine($"2: Payment Prove (0x{GetReceiptCase(0x4652000000000002):X})");
            Console.WriteLine($"3: Invoice (0x{GetReceiptCase(0x4652000000000003):X})");
            Console.WriteLine($"4: Shift Receipt (0x{GetReceiptCase(0x4652000000000004):X})");
            Console.WriteLine($"5: Daily Receipt (0x{GetReceiptCase(0x4652000000000005):X})");
            Console.WriteLine($"6: Monthly Receipt (0x{GetReceiptCase(0x4652000000000006):X})");
            Console.WriteLine($"7: Yearly Receipt (0x{GetReceiptCase(0x4652000000000007):X})");
            Console.WriteLine($"8: Bill (0x{GetReceiptCase(0x4652000000000008):X})");
            Console.WriteLine($"9: Delivery Note Receipt (0x{GetReceiptCase(0x4652000000000009):X})");
            Console.WriteLine($"10: Cash Deposit (0x{GetReceiptCase(0x465200000000000A):X})");
            Console.WriteLine($"11: Payout Receipt (0x{GetReceiptCase(0x465200000000000B):X})");
            Console.WriteLine($"12: Payment Transfer (0x{GetReceiptCase(0x465200000000000C):X})");
            Console.WriteLine($"13: Internal Receipt (0x{GetReceiptCase(0x465200000000000D):X})");
            Console.WriteLine($"14: Foreign Sale Receipt (0x{GetReceiptCase(0x465200000000000E):X})");
            Console.WriteLine($"15: Zero Receipt (0x{GetReceiptCase(0x465200000000000F):X})");
            Console.WriteLine($"16: Start Receipt (0x{GetReceiptCase(0x4652000000000010):X})");
            Console.WriteLine($"17: Stop Receipt (0x{GetReceiptCase(0x4652000000000011):X})");
            Console.WriteLine($"18: Log (0x{GetReceiptCase(0x4652000000000012):X})");
            Console.WriteLine($"19: Audit Receipt (0x{GetReceiptCase(0x4652000000000013):X})");
            Console.WriteLine($"20: Protocol Receipt (0x{GetReceiptCase(0x4652000000000014):X})");
            Console.WriteLine($"21: Archive (0x{GetReceiptCase(0x4652000000000015):X})");
            Console.WriteLine($"22: Copy (0x{GetReceiptCase(0x4652000000000016):X})");

            Console.WriteLine("23: Turn on/off Training mode");

            Console.WriteLine("24: French Journals");
            Console.WriteLine($"25: Number of Tickets (max {MAX_GENERATED_RECEIPT_COUNT})");


			Console.WriteLine("exit: Exit program");

            string input = Console.ReadLine();

            Command(input);
        }

        static long GetReceiptCase(long baseReceiptCase)
        {
            return isTraining ? baseReceiptCase | 0x0000000000020000 : baseReceiptCase;
        }

        static void Command(string input)
        {
            if (input.ToLower().StartsWith("exit"))
            {
                var stream = proxy.Journal(0xFE, 0, DateTime.UtcNow.Ticks);
                var sr = new System.IO.StreamReader(stream);

                Console.WriteLine("{0:G} ========== Default Journal ==========", DateTime.Now);

                Console.WriteLine(sr.ReadToEnd());

                Environment.Exit(0);
            }

            Random r = new Random((int)DateTime.Now.Ticks);

            int inputInt;
            if (!int.TryParse(input, out inputInt))
            {
                Console.WriteLine($"\"{input}\" not recognised.");
                return;
            }

            if (inputInt == 1)
            {
                var req = TicketRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Ticket request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 2)
            {
                var req = PaymentProveRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Payment Prove request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 3)
            {
                var req = InvoiceRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Invoice request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 4)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000004));
                
                Console.WriteLine("{0:G} Shift Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 5)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000005));
                
                Console.WriteLine("{0:G} Daily Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 6)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000006));
                
                Console.WriteLine("{0:G} Monthly Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 7)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000007));
                
                Console.WriteLine("{0:G} Yearly Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 8)
            {
                var req = BillRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Bill request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));

                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 9)
            {
                var req = DeliveryNoteRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Delivery Note request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));

                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 10)
            {
                var req = CashDepositRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Cash Deposit request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));

                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 11)
            {
                var req = PayoutRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Payout request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 12)
            {
                var req = PaymentTransferRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Payment Transfer request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 13)
            {
                var req = InternalRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Internal request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 14)
            {
                var req = ForeignSaleRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                
                Console.WriteLine("{0:G} Foreign Sale request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 15)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x465200000000000F));
                
                Console.WriteLine("{0:G} Zero Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 16)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000010));
                
                Console.WriteLine("{0:G} Start Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 17)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000011));
                
                Console.WriteLine("{0:G} Stop Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 18)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000012));
                
                Console.WriteLine("{0:G} Log Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 19)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000013));
                
                Console.WriteLine("{0:G} Audit Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 20)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000014));
                
                Console.WriteLine("{0:G} Protocol Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 21)
            {
                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000015));
                
                Console.WriteLine("{0:G} Archive Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 22)
            {
                Console.WriteLine("Enter the receipt reference for the requested copy: [1]");

                var req = ZeroReceiptRequest(++i, cashBoxId, GetReceiptCase(0x4652000000000016));
                
                req.ftReceiptCaseData = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(req.ftReceiptCaseData))
                    req.ftReceiptCaseData = "1";

                Console.WriteLine("{0:G} Copy Receipt request: {1}", DateTime.Now, JsonConvert.SerializeObject(req));
                var resp = proxy.Sign(req);

                Response(resp);
            }
            else if (inputInt == 23)
            {
                isTraining = !isTraining;
            }
            else if (inputInt == 24)
            {
                string path = $"c:\\temp";

                Console.Write("{0:G} Journal export folder ({1})", DateTime.Now, path);

                string inputPath = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputPath))
                {
                    path = inputPath;
                }

                ExportJournal(path, 0x4652000000000001, "Ticket");
                ExportJournal(path, 0x4652000000000002, "Payment Prove");
                ExportJournal(path, 0x4652000000000003, "Invoice");
                ExportJournal(path, 0x4652000000000004, "Grand Total");
                ExportJournal(path, 0x4652000000000007, "Bill");
                ExportJournal(path, 0x4652000000000008, "Archive");
                ExportJournal(path, 0x4652000000000009, "Log");
                ExportJournal(path, 0x465200000000000A, "Copy");
                ExportJournal(path, 0x465200000000000B, "Training");

            }
            else if (inputInt >= 25 && inputInt < MAX_GENERATED_RECEIPT_COUNT)
            {
                long max = long.MinValue;
                long min = long.MaxValue;
                long sum = 0;
                int n = 0;
                while (n++ < inputInt)
                {
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    var req = TicketRequest(++i, decimal.Round((decimal)(r.NextDouble() * 100), 2), decimal.Round((decimal)(r.NextDouble() * 100), 2), cashBoxId);
                    
                    var resp = proxy.Sign(req);
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
                Console.WriteLine($"\"{input}\" not recognised.");
            }

            Menu();
        }

        internal static void ExportJournal(string path, long type, string name)
        {
            string filename = System.IO.Path.Combine(path, $"{cashBoxId}_{name}_{DateTime.UtcNow.Ticks}.csv");
            using (var file = System.IO.File.Open(filename, System.IO.FileMode.Create))
            {
                using (var journal = proxy.Journal(type, 0, 0))
                {
                    journal.CopyTo(file);
                }

                Console.WriteLine($"{DateTime.Now:G} {name} Journal exported to {filename} ({((decimal)file.Length) / 1024m / 1024m:0.00}Mb)");
            }
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
                }
            }
            else
            {
                Console.WriteLine("null-result!!!");
            }
        }

        internal static ReceiptRequest TicketRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x4652000000000001),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest InvoiceRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x4652000000000003),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000001,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000001,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest BillRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x4652000000000008),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000001,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000001,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000011,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Payment compensation"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest TrainingRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x465200000000000B),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest DeliveryNoteRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x4652000000000009),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest InternalRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x465200000000000D),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000003,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    }
                }
            };

            return reqdata;
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

        internal static ReceiptRequest PaymentProveRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {
            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x4652000000000002),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,
                cbChargeItems = new ChargeItem[] { },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    },
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000011,
                        Amount=-amount1-amount2,
                        Quantity=1.0m,
                        Description="Payment compensation"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest CashDepositRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {
            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x465200000000000A),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,
                cbChargeItems = new ChargeItem[] { },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    },
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000011,
                        Amount=-amount1-amount2,
                        Quantity=1.0m,
                        Description="Payment compensation"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest PayoutRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {
            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x465200000000000B),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,
                cbChargeItems = new ChargeItem[] { },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    },
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000011,
                        Amount=-amount1-amount2,
                        Quantity=1.0m,
                        Description="Payment compensation"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest PaymentTransferRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {
            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x465200000000000C),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,
                cbChargeItems = new ChargeItem[] { },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000001,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Cash"
                    },
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000011,
                        Amount=-amount1-amount2,
                        Quantity=1.0m,
                        Description="Payment compensation"
                    }
                }
            };

            return reqdata;
        }

        internal static ReceiptRequest ForeignSaleRequest(int n, decimal amount1 = 4.8m, decimal amount2 = 3.3m, string cashBoxId = "")
        {

            var reqdata = new ReceiptRequest()
            {
                ftCashBoxID = cashBoxId,
                cbTerminalID = "1",
                ftReceiptCase = GetReceiptCase(0x465200000000000E),
                cbReceiptReference = n.ToString(),
                cbReceiptMoment = DateTime.UtcNow,

                cbChargeItems = new ChargeItem[]  {
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000001,
                         ProductNumber="1",
                         Description="Article 1",
                         Quantity=1.0m,
                         VATRate=20.0m,
                         Amount=amount1
                    },
                    new ChargeItem()
                    {
                        ftChargeItemCase=0x4652000000000001,
                        ProductNumber="2",
                        Description="Article 2",
                        Quantity=1.0m,
                        VATRate=20.0m,
                        Amount=amount2
                    }
                },
                cbPayItems = new PayItem[]                {
                    new PayItem()
                    {
                        ftPayItemCase=0x4652000000000011,
                        Amount=amount1+amount2,
                        Quantity=1.0m,
                        Description="Payment compensation"
                    }
                }
            };

            return reqdata;
        }
    }
}
