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
        public const string FileExtension_JSON = "json";
        public const string FileExtension_CSV = "csv";
        static string url;
        static string cashBoxId;
        static List<MenuItem> menuItems = new List<MenuItem>() {
            new MenuItem() { Number = 1, Description = "ActionJournal", Type = 0x01, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 2, Description = "ReceiptJournal", Type = 0x02, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 3, Description = "QueueItem", Type = 0x03, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 4, Description = "Journal AT", Type = 0x4154, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 5, Description = "Journal DE", Type = 0x4445, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 6, Description = "Journal FR", Type = 0x4652, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 7, Description = "AT RKSV-DEP export", Type = 0x4154000000000001, InitialSequence = "\"Belege-kompakt\": [", FinalSequence = "]\r\n    }\r\n  ]\r\n}", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 8, Description = "FR LNE Ticket export", Type = 0x4652000000000001, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 9, Description = "FR LNE Payment Prove export", Type = 0x4652000000000002, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 10, Description = "FR LNE Invoice export", Type = 0x4652000000000003, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 11, Description = "FR LNE Grand Total export", Type = 0x4652000000000004, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 12, Description = "FR LNE Bill export", Type = 0x4652000000000007, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 13, Description = "FR LNE Archive export", Type = 0x4652000000000008, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 14, Description = "FR LNE Log export", Type = 0x4652000000000009, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 15, Description = "FR LNE Copy export", Type = 0x465200000000000A, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 16, Description = "FR LNE Training export", Type = 0x465200000000000B, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 17, Description = "Full Status Information", Type = 0xFF, InitialSequence = "", FinalSequence = "", NeedsChunking = false, FileExtension = FileExtension_JSON, ChunkSeparator = " , " }
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
            foreach (var item in menuItems)
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
            var exportFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), $"{cashBoxId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{item.Description}_0x{item.Type:X}.{item.FileExtension}");

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
            if (item.Type >> 48 != 0 && to < 0)
            {
                System.IO.Stream chunkEntityStream = null;

                if ((Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) && url.StartsWith("http://"))
                {
                    chunkEntityStream = fiskaltrust.ifPOS.Utilities.MonoJournalSOAP(url, cashBoxId, string.Empty, item.Type >> 48, from, to, timeout * 1000);
                }
                else
                {
                    chunkEntityStream = proxy.Journal(item.Type >> 48, from, to);
                }

                using (var stream = chunkEntityStream)
                {
                    try
                    {
                        string requestJournalText = (new System.IO.StreamReader(stream)).ReadToEnd().Replace("\n", "").Replace("\r", "");
                        dynamic itemArray = JsonConvert.DeserializeObject(requestJournalText);
                        foreach (var i in itemArray)
                        {
                            to = Math.Max(to, (long)i.TimeStamp);
                        }
                    }
                    catch { }
                    stream.Close();
                }
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

            using (var stream = entityStream)
            {
                string requestJournalText = (new System.IO.StreamReader(stream)).ReadToEnd();

                if (to < 0)
                {
                    try
                    {
                        dynamic itemArray = JsonConvert.DeserializeObject(requestJournalText);
                        foreach (var i in itemArray)
                        {
                            to = Math.Max(to, (long)i.TimeStamp);
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(exportFilePath))
                {
                    using (var exportFile = new System.IO.FileStream(exportFilePath, System.IO.FileMode.Append))
                    {
                        string cutData = string.Empty;

                        if (to < 0)
                        {
                            if (exportFile.Position <= 0)
                            {
                                cutData = requestJournalText;
                            }
                            else
                            {
                                cutData = item.FinalSequence;
                            }
                        }
                        else
                        {
                            int initialPos = int.MinValue;
                            int finalPos = int.MinValue;

                            if (item.FileExtension == FileExtension_JSON)
                            {
                                requestJournalText = requestJournalText.Replace("\n", "").Replace("\r", "");
                                initialPos = requestJournalText.IndexOf(item.InitialSequence.Trim().Replace("\n", "").Replace("\r", ""));
                                finalPos = requestJournalText.LastIndexOf(item.FinalSequence.Trim().Replace("\n", "").Replace("\r", ""));
                            }
                            else
                            {
                                initialPos = requestJournalText.IndexOf(item.InitialSequence);
                                finalPos = requestJournalText.LastIndexOf(item.FinalSequence);
                            }

                            if (writeInitialSequence)
                            {
                                initialPos = 0;
                            }
                            else
                            {
                                if (item.FileExtension == FileExtension_JSON)
                                {
                                    initialPos += item.InitialSequence.Trim().Length;
                                }
                                else
                                {
                                    initialPos += item.InitialSequence.Length;
                                }
                                if (exportFile.Position > 0)
                                    cutData = item.ChunkSeparator;
                            }

                            if (writeFinalSequence || item.FinalSequence.Length <= 0)
                            {
                                finalPos = requestJournalText.Length;
                            }

                            cutData += requestJournalText.Substring(initialPos, finalPos - initialPos);

                            if (cutData.EndsWith(item.ChunkSeparator))
                                cutData = cutData.Substring(0, cutData.Length - item.ChunkSeparator.Length);
                        }

                        var arrayCutData = System.Text.Encoding.UTF8.GetBytes(cutData);
                        exportFile.Write(arrayCutData, 0, arrayCutData.Length);
                        exportFile.Flush();
                        exportFile.Close();
                    }
                }
                stream.Close();
            }

            return to;
        }

        class MenuItem
        {
            public int Number { get; set; }
            public string Description { get; set; }
            public long Type { get; set; }
            public string InitialSequence { get; set; }
            public string FinalSequence { get; set; }
            public bool NeedsChunking { get; set; }
            public string FileExtension { get; set; }
            public string ChunkSeparator { get; set; }
        }
    }
}
