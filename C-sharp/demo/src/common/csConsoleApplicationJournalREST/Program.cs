using fiskaltrust.ifPOS.v0;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace csConsoleApplicationJournalREST
{
    class Program
    {
        public const string FileExtension_JSON = "json";
        public const string FileExtension_CSV = "csv";
        public const long MinimunRecordsToBeDownloaded = 50;
        public const long DownloadingRecordIncrease = 50;
        private static string url = "https://signaturcloud-sandbox.fiskaltrust.at/";
        private static Guid cashboxid = Guid.Empty;
        private static string accesstoken;
        private static bool json;
        private static long readcount;
        private static int timeout;
        private static string serviceversion;

        static List<MenuItem> menuItems = new List<MenuItem>() {
            new MenuItem() { Number = 1, Description = "ActionJournal", Type = 0x01, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 2, Description = "ReceiptJournal", Type = 0x02, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 3, Description = "QueueItem", Type = 0x03, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 4, Description = "Journal AT", Type = 0x4154, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 5, Description = "Journal DE", Type = 0x4445, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 6, Description = "Journal FR", Type = 0x4652, InitialSequence = "[", FinalSequence = "]", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 7, Description = "AT RKSV-DEP export", Type = 0x4154000000000001, InitialSequence = "\"Belege-kompakt\": [", FinalSequence = "]\r\n    }\r\n  ]\r\n}", NeedsChunking = true, FileExtension = FileExtension_JSON, ChunkSeparator = " , " },
            new MenuItem() { Number = 8, Description = "FR Ticket export", Type = 0x4652000000000001, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 9, Description = "FR Payment Prove export", Type = 0x4652000000000002, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 10, Description = "FR Invoice export", Type = 0x4652000000000003, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 11, Description = "FR Grand Total export", Type = 0x4652000000000004, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 12, Description = "FR Bill export", Type = 0x4652000000000007, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 13, Description = "FR Archive export", Type = 0x4652000000000008, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 14, Description = "FR Log export", Type = 0x4652000000000009, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 15, Description = "FR Copy export", Type = 0x465200000000000A, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 16, Description = "FR Training export", Type = 0x465200000000000B, InitialSequence = Environment.NewLine, FinalSequence = "", NeedsChunking = true, FileExtension = FileExtension_CSV, ChunkSeparator = Environment.NewLine },
            new MenuItem() { Number = 17, Description = "Full Status Information", Type = 0xFF, InitialSequence = "", FinalSequence = "", NeedsChunking = false, FileExtension = FileExtension_JSON, ChunkSeparator = " , " }
        };

        static void Main(string[] args)
        {

            ServicePointManager.DefaultConnectionLimit = 65535;

            var options = ProgramOptions.GetOptionsFromCommandLine(args);

            url = options.url;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            cashboxid = options.cashboxid;
            accesstoken = options.accesstoken;
            json = (bool)options.json;
            readcount = options.readcount;
            timeout = options.timeout;
            serviceversion = options.serviceversion;

            // use echo for communication test
            if (json)
            {
                var message = EchoRESTJSON("message", url, cashboxid, accesstoken);
                if (message != "message") throw new Exception("echo failed");
            }
            else
            {
                var message = EchoRESTXML("message", url, cashboxid, accesstoken);
                if (message != "message") throw new Exception("echo failed");
            }

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
            var exportFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), $"{cashboxid}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{item.Description}_0x{item.Type:X}.{item.FileExtension}");

            if (item.NeedsChunking)
            {
                long pointer = 0;
                long readCount = readcount;
                long maxReadCount = readcount * 10 > long.MaxValue ? long.MaxValue : readcount * 10;

                do
                {
                    try
                    {
                        pointer = WriteJournal(item, pointer, -readCount, exportFilePath, pointer == 0, false) + 1;

                        if (readCount < maxReadCount)
                        {
                            readCount += DownloadingRecordIncrease;
                        }
                    }
                    catch (Exception ex)
                    {
                        readCount = (readCount / 2) + 1;
                        if (readcount < MinimunRecordsToBeDownloaded)
                        {
                            Console.WriteLine($"Journal {item.Description} cannot be downloaded. Reported error message: {ex.Message}");
                            return;
                        }
                    }
                } while (pointer >= 0);
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

                if (json)
                {
                    chunkEntityStream = JournalRESTJSON(item.Type >> 48, from, to, url, cashboxid, accesstoken);
                }
                else
                {
                    chunkEntityStream = JournalRESTXML(item.Type >> 48, from, to, url, cashboxid, accesstoken);
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

            if (json)
            {
                entityStream = JournalRESTJSON(item.Type, from, to, url, cashboxid, accesstoken);
            }
            else
            {
                entityStream = JournalRESTXML(item.Type, from, to, url, cashboxid, accesstoken);
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

        private static string SerializeToJsonString<T>(T obj)
        {
            string result;
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var jw = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
                serializer.Serialize(jw, obj);
                jw.Flush();
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                    result = sr.ReadToEnd();
            }
            return result;
        }

        public static T DeserializeFromJsonStream<T>(Stream stream)
        {
            T obj;

            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
                obj = serializer.Deserialize<T>(jr);
            }
            return obj;
        }

        public static string EchoRESTXML(string message, string url, Guid cashboxid, string accesstoken)
        {
            var ms = new System.IO.MemoryStream();
            var serializer = new DataContractSerializer(typeof(string));

            serializer.WriteObject(ms, message);
            Console.WriteLine("{0:G} Request {1}", DateTime.Now, Encoding.UTF8.GetString(ms.ToArray()));

            var webreq = (HttpWebRequest)HttpWebRequest.Create(url + "/xml/echo");
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
                return resp;
            }
            else
            {
                Console.WriteLine("{0:G} {1} {2}", DateTime.Now, webresp.StatusCode, webresp.StatusDescription);
            }
            return null;
        }

        public static Stream JournalRESTXML(long ftJournalType, long from, long to, string url, Guid cashboxid, string accesstoken)
        {
            Console.WriteLine("{0:G} Journal request", DateTime.Now);

            var webreq = (HttpWebRequest)HttpWebRequest.Create($"{url}/xml/journal?type={ftJournalType}&from={from}&to={to}");
            webreq.Method = "POST";
            webreq.ContentType = "application/xml;charset=utf-8";
            webreq.ContentLength = 0;
            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                using (var respStream = webresp.GetResponseStream())
                {
                    var ms = new System.IO.MemoryStream();
                    webresp.GetResponseStream().CopyTo(ms);
                    Console.WriteLine("{0:G} journal response len {1}", DateTime.Now, ms.Length); // to show journal text use text instead of text.length

                    ms.Position = 0;
                    return ms;
                }
            }
            else
            {
                Console.WriteLine("{0:G} {1} {2}", DateTime.Now, webresp.StatusCode, webresp.StatusDescription);
            }

            return null;
        }

        public static string EchoRESTJSON(string message, string url, Guid cashboxid, string accesstoken)
        {
            var webreq = (HttpWebRequest)HttpWebRequest.Create(url + "/json/echo");
            webreq.Method = "POST";
            webreq.ContentType = "application/json;charset=utf-8";
            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);

            var reqecho = Encoding.UTF8.GetBytes(SerializeToJsonString(message));
            webreq.ContentLength = reqecho.Length;
            using (var reqStream = webreq.GetRequestStream())
            {
                reqStream.Write(reqecho, 0, reqecho.Length);
            }

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                var resp = DeserializeFromJsonStream<string>(webresp.GetResponseStream());
                Console.WriteLine("{0:G} Echo {1}", DateTime.Now, resp);
                return resp;
            }
            else
            {
                Console.WriteLine("{0:G} {1} {2}", DateTime.Now, webresp.StatusCode, webresp.StatusDescription);
            }
            return null;
        }

        public static Stream JournalRESTJSON(long ftJournalType, long from, long to, string url, Guid cashboxid, string accesstoken)
        {
            Console.WriteLine("{0:G} Journal request", DateTime.Now);

            var webreq = (HttpWebRequest)HttpWebRequest.Create($"{url}/json/journal?type={ftJournalType}&from={from}&to={to}");
            webreq.Method = "POST";
            webreq.ContentType = "application/json;charset=utf-8";
            webreq.ContentLength = 0;
            webreq.Headers.Add("cashboxid", cashboxid.ToString());
            webreq.Headers.Add("accesstoken", accesstoken);
            webreq.Headers.Add("service-version", serviceversion);
            webreq.Timeout = timeout;

            var webresp = (HttpWebResponse)webreq.GetResponse();
            if (webresp.StatusCode == HttpStatusCode.OK)
            {
                using (var respStream = webresp.GetResponseStream())
                {
                    var ms = new System.IO.MemoryStream();
                    webresp.GetResponseStream().CopyTo(ms);
                    Console.WriteLine("{0:G} journal response len {1}", DateTime.Now, ms.Length); // to show journal text use text instead of text.length

                    ms.Position = 0;
                    return ms;
                }
            }
            else
            {
                Console.WriteLine("{0:G} {1} {2}", DateTime.Now, webresp.StatusCode, webresp.StatusDescription);
            }

            return null;
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
