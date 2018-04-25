using fiskaltrust.ifPOS.v0;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace csConsoleApplicationJournalSOAP
{
    class Program
    {
        static string url;
        static string cashBoxId;
        static List<MenuItem> menuItems = new List<MenuItem>() {
            new MenuItem() { Number = 1, Description = "ActionJournal", Type = 0x01, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true },
            new MenuItem() { Number = 2, Description = "ReceiptJournal", Type = 0x02, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true },
            new MenuItem() { Number = 3, Description = "QueueItem", Type = 0x03, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true },
            new MenuItem() { Number = 4, Description = "Journal AT", Type = 0x4154, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true },
            new MenuItem() { Number = 5, Description = "Journal DE", Type = 0x4445, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true },
            new MenuItem() { Number = 6, Description = "Journal FR", Type = 0x4652, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true },
            new MenuItem() { Number = 7, Description = "AT RKSV-DEP export", Type = 0x4154000000000001, InitialSequence = "\"Belege-kompakt\": [", FinalSequence = "]\r\n    }\r\n  ]\r\n}", NeedsChunking = true },
            new MenuItem() { Number = 8, Description = "Full Status Information", Type = 0xFF, InitialSequence = "", FinalSequence = "", NeedsChunking = false }
        };

        static IPOS proxy = null;
        static int timeout = 180; //3 minutes

        static void Main(string[] args)
        {

            ServicePointManager.DefaultConnectionLimit = 65535;

            Console.Write("fiskaltrust-service-url:");
            url = Console.ReadLine();

            Console.Write("cashboxid:");
            cashBoxId = Console.ReadLine();

            Guid _tempCashBoxID;
            if (!Guid.TryParse(cashBoxId, out _tempCashBoxID))
            {
                throw new ArgumentException("cashboxid is not a guid!");
            }

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
            foreach(var item in menuItems)
            {
                Console.WriteLine($"{item.Number}: {item.Description} (0x{item.Type:X})");
            }

            Console.WriteLine("exit: Exit Program");

            string input = Console.ReadLine();

            Command(input);
        }

        static void Command(string input)
        {
            if (input.ToLower().StartsWith("exit"))
            {
                Environment.Exit(0);
            }

            int inputInt;
            if (!int.TryParse(input, out inputInt) || !menuItems.Select(mi => mi.Number).Contains(inputInt))
            {
                Console.WriteLine($"\"{input}\" not recognised.");
                return;
            }

            var menuItem = menuItems.Where(mi => mi.Number == inputInt).FirstOrDefault();

            Journal(menuItem);
        }

        static void Journal(MenuItem item)
        {
            var exportFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) , $"{cashBoxId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{item.Description}_0x{item.Type:X}.json");

            if (item.NeedsChunking)
            {
                long pointer = 0;
                int readCount = 1000;
                int maxReadCount = 10000;

                do
                {
                    try
                    {
                        pointer = WriteJournal(item, pointer, -readCount, exportFilePath, pointer == 0, false) + 1;
                             
                        if (readCount < maxReadCount)
                        {
                            readCount += 50;
                        }
                    }
                    catch (Exception x)
                    {
                        readCount = (readCount / 2) + 1;
                    }
                } while (pointer > 0);
            }
            else
            {
                WriteJournal(item, 0, 0, exportFilePath, true, true);
            }

            Console.WriteLine($"Journal {item.Description} written on file {exportFilePath}");
        }

        static long WriteJournal(MenuItem item, long from, long to, string exportFilePath, bool writeInitialSequence, bool writeFinalSequence)
        {
            if (item.Type >> 48 != 0)
            {
                var newto = WriteJournal(new MenuItem() { Type = item.Type >> 48 }, from, to, null, false, false);
                if (newto > 0)
                    to = newto;
            }

            System.IO.Stream entityStream = null;

            if ((Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) && url.StartsWith("http://"))
            {
                entityStream = fiskaltrust.ifPOS.Utilities.MonoJournalSOAP(url, cashBoxId, string.Empty, item.Type, from, to, timeout * 1000);
            }
            else
            {
                entityStream = proxy.Journal(item.Type, from, to);
            }

            var result = long.MinValue;

            using (var stream = entityStream)
            {
                string requestJournalText = (new System.IO.StreamReader(stream)).ReadToEnd().Replace("\n", "").Replace("\r", "");
                if (to < 0)
                {
                    try
                    {
                        dynamic itemArray = JsonConvert.DeserializeObject(requestJournalText);
                        var finalTimestamp = itemArray[itemArray.Count - 1].TimeStamp;
                        result = finalTimestamp;
                    }
                    catch { }
                }
                else
                {
                    result = to;
                }

                if (!string.IsNullOrWhiteSpace(exportFilePath))
                {
                    var cutData = string.Empty;
                    var initialPos = requestJournalText.IndexOf(item.InitialSequence.Trim().Replace("\n", "").Replace("\r", ""));
                    var finalPos = requestJournalText.LastIndexOf(item.FinalSequence.Trim().Replace("\n", "").Replace("\r", ""));

                    if (writeInitialSequence)
                    {
                        initialPos = 0;
                    }
                    else
                    {
                        initialPos += item.InitialSequence.Trim().Length;
                        if (result > 0)
                            cutData = " , ";
                    }

                    if (writeFinalSequence || result < 0)
                    {
                        finalPos = requestJournalText.Length;
                    }

                    cutData += requestJournalText.Substring(initialPos, finalPos - initialPos);
                    var arrayCutData = System.Text.Encoding.UTF8.GetBytes(cutData);
                    using (var exportFile = new System.IO.FileStream(exportFilePath, System.IO.FileMode.Append))
                    {
                        exportFile.Write(arrayCutData, 0, arrayCutData.Length);
                        exportFile.Flush();
                        exportFile.Close();
                    }
                }
                stream.Close();
            }

            return result;
        }

        class MenuItem
        {
            public int Number { get; set; }
            public string Description { get; set; }
            public long Type { get; set; }
            public string InitialSequence { get; set; }
            public string FinalSequence { get; set; }
            public bool NeedsChunking { get; set; }
        }
    }
}
