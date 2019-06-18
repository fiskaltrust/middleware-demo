using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.ServiceModel;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace csClassLibraryCOMInterop
{

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("CB7A5038-A2A0-4462-944D-7B429E43B20B")]
    public class ft
    {
        private const int _round_digits = 10;

        public string serviceUrl { get; set; }

        public ft()
        {
            //serviceUrl = "http://localhost:1201/fiskaltrust/POS";
        }

        private fiskaltrust.ifPOS.v0.IPOS proxy = null;

        public bool Connected
        {
            get
            {
                return proxy != null;
            }
        }

        //public bool Connect(string url)
        //{
        //    serviceUrl = url;
        //    return Connect();
        //}

        public bool Connect()
        {
            try
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                var endpoint = new EndpointAddress(serviceUrl);
                var factory = new ChannelFactory<fiskaltrust.ifPOS.v0.IPOS>(binding, endpoint);
                proxy = factory.CreateChannel();
                proxy.Echo(DateTime.Now.ToString());
            }
            catch (Exception)
            {
                proxy = null;
            }
            return Connected;
        }

        public void Disconnect()
        {
            if (Connected)
            {
                proxy = null;
            }
        }


        public string jsonSign(string request)
        {
            if (!Connected) throw new Exception("Not Connected");
            try
            {
                var jsonSettings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
                var req = JsonConvert.DeserializeObject<fiskaltrust.ifPOS.v0.ReceiptRequest>(request, jsonSettings);
                var resp = proxy.Sign(req);
                return JsonConvert.SerializeObject(resp, jsonSettings);
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        public Signer signer
        {
            get; private set;
        }

        public void signerInit()
        {
            if (!Connected) Connect();
            signer = new Signer(proxy);
        }


        [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
        [Guid("8F3E0628-3A7A-4E06-BD32-8F8633F77D13")]
        public class Signer
        {
            private fiskaltrust.ifPOS.v0.IPOS _proxy = null;
            private fiskaltrust.ifPOS.v0.ReceiptRequest _requestData;
            private fiskaltrust.ifPOS.v0.ReceiptResponse _responseData;
            private SignResponse _signData;

            private JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };


            public Signer(fiskaltrust.ifPOS.v0.IPOS proxy)
            {
                _proxy = proxy;
                Reset();
            }

            public void Reset()
            {
                _requestData = new fiskaltrust.ifPOS.v0.ReceiptRequest();
                _responseData = null;
                _signData = null;
                LastError = string.Empty;
            }

            public bool Sign()
            {
                try
                {
                    _requestData.cbChargeItems = _requestChargeItems.ToArray();
                    _requestData.cbPayItems = _requestPayItems.ToArray();

                    _responseData = _proxy.Sign(_requestData);
                    _signData = new SignResponse(_responseData);

                    return true;
                }
                catch (Exception x)
                {
                    LastError = x.Message;
                    return false;
                }
            }

            public string LastError
            {
                get; private set;
            }

            public string requestJson
            {
                get
                {
                    return JsonConvert.SerializeObject(_requestData, jsonSettings);
                }
            }

            public string responseJson
            {
                get
                {
                    return JsonConvert.SerializeObject(_responseData, jsonSettings);
                }
            }

            public string ftCashBoxID
            {
                set
                {
                    _requestData.ftCashBoxID = value;
                }
            }

            public string cbTerminalID
            {
                set
                {
                    _requestData.cbTerminalID = value;
                }
            }

            public string cbReceiptReference
            {
                set
                {
                    _requestData.cbReceiptReference = value;
                }
            }

            public DateTime cbReceiptMoment
            {
                set
                {
                    _requestData.cbReceiptMoment = value;
                }
            }

            public string ftReceiptCase
            {
                set
                {
                    _requestData.ftReceiptCase = Convert.ToInt64(value, 16);
                }
            }

            public string ftReceiptCaseData
            {
                set
                {
                    _requestData.ftReceiptCaseData = value;
                }
            }

            public double cbReceiptAmount
            {
                set
                {
                    _requestData.cbReceiptAmount = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            private List<fiskaltrust.ifPOS.v0.ChargeItem> _requestChargeItems = new List<fiskaltrust.ifPOS.v0.ChargeItem>();
            private List<fiskaltrust.ifPOS.v0.PayItem> _requestPayItems = new List<fiskaltrust.ifPOS.v0.PayItem>();

            [ComVisible(false)]
            public void AddftChargeItem(fiskaltrust.ifPOS.v0.ChargeItem item)
            {
                _requestChargeItems.Add(item);
            }

            public void AddChargeItem(ChargeItem item)
            {
                _requestChargeItems.Add(item.ftChageItem());
            }

            public void AddChargeItemValues(double Quantitiy, double Amount, string Description, double VATRate, string ftChargeItemCase)
            {
                var item = new ChargeItem();
                item.Quantity = Quantitiy;
                item.Amount = Amount;
                item.Description = Description;
                item.VATRate = VATRate;
                item.ftChargeItemCase = ftChargeItemCase;
                _requestChargeItems.Add(item.ftChageItem());
            }

            public void RemoveChargeItem(int index)
            {
                _requestChargeItems.Remove(_requestChargeItems[index]);
            }

            public void ClearChageItems()
            {
                _requestChargeItems.Clear();
            }

            public int ChargeItemsLen
            {
                get
                {
                    return _requestChargeItems.Count;
                }
            }

            public ChargeItem GetChargeItem(int index)
            {
                return new ChargeItem(_requestChargeItems[index]);
            }

            [ComVisible(false)]
            public void AddftPayItem(fiskaltrust.ifPOS.v0.PayItem item)
            {
                _requestPayItems.Add(item);
            }

            public void AddPayItem(PayItem item)
            {
                _requestPayItems.Add(item.ftPayItem());
            }

            public void AddPayItemValues(double Quantitiy, double Amount, string Description, string ftPayItemCase)
            {
                var item = new PayItem();
                item.Quantity = Quantitiy;
                item.Amount = Amount;
                item.Description = Description;
                item.ftPayItemCase = ftPayItemCase;
                _requestPayItems.Add(item.ftPayItem());
            }

            public void RemovePayItem(int index)
            {
                _requestPayItems.Remove(_requestPayItems[index]);
            }

            public void ClearPayItems()
            {
                _requestPayItems.Clear();
            }

            public int PayItemsLen
            {
                get
                {
                    return _requestPayItems.Count;
                }
            }

            public PayItem GetPayItem(int index)
            {
                return new PayItem(_requestPayItems[index]);
            }

            public SignResponse Response
            {
                get
                {
                    return _signData;
                }
            }

        }


        [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
        [Guid("0A349FC9-6685-4737-A4D4-92E6CC513348")]
        public class ChargeItem
        {
            private fiskaltrust.ifPOS.v0.ChargeItem _item;
            public ChargeItem()
            {
                _item = new fiskaltrust.ifPOS.v0.ChargeItem();
            }

            public ChargeItem(fiskaltrust.ifPOS.v0.ChargeItem item)
            {
                _item = item;
            }


            public double Quantity
            {
                get
                {
                    return Convert.ToDouble(_item.Quantity);
                }
                set
                {
                    _item.Quantity = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            public double Amount
            {
                get
                {
                    return Convert.ToDouble(_item.Amount);
                }
                set
                {
                    _item.Amount = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            public string Description
            {
                get
                {
                    return _item.Description;
                }
                set
                {
                    _item.Description = value;
                }
            }

            public double VATRate
            {
                get
                {
                    return Convert.ToDouble(_item.VATRate);
                }
                set
                {
                    _item.VATRate = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            public string ftChargeItemCase
            {
                get
                {
                    return Convert.ToString(_item.ftChargeItemCase, 16);
                }
                set
                {
                    try
                    {
                        _item.ftChargeItemCase = Convert.ToInt64(value, 16);
                    }
                    catch (Exception)
                    {
                        _item.ftChargeItemCase = 0;
                    }
                }
            }

            public string ftChargeItemCaseData
            {
                get
                {
                    return _item.ftChargeItemCaseData;
                }
                set
                {
                    _item.ftChargeItemCaseData = value;
                }
            }

            public double VATAmount
            {
                get
                {
                    if (_item.VATAmount.HasValue) return Convert.ToDouble(_item.VATAmount);
                    else return double.NaN;
                }
                set
                {
                    _item.VATAmount = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            [ComVisible(false)]
            public fiskaltrust.ifPOS.v0.ChargeItem ftChageItem()
            {
                return _item;
            }

        }

        [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
        [Guid("CBBF5596-1BB3-4D30-BD1F-3F643B2DF5D3")]
        public class PayItem
        {
            private fiskaltrust.ifPOS.v0.PayItem _item;
            public PayItem()
            {
                _item = new fiskaltrust.ifPOS.v0.PayItem();
            }

            public PayItem(fiskaltrust.ifPOS.v0.PayItem item)
            {
                _item = item;
            }

            public double Quantity
            {
                get
                {
                    return Convert.ToDouble(_item.Quantity);
                }
                set
                {
                    _item.Quantity = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            public double Amount
            {
                get
                {
                    return Convert.ToDouble(_item.Amount);
                }
                set
                {
                    _item.Amount = Convert.ToDecimal(Math.Round(value, _round_digits));
                }
            }

            public string Description
            {
                get
                {
                    return _item.Description;
                }
                set
                {
                    _item.Description = value;
                }
            }

            public string ftPayItemCase
            {
                get
                {
                    return Convert.ToString(_item.ftPayItemCase, 16);
                }
                set
                {
                    try
                    {
                        _item.ftPayItemCase = Convert.ToInt64(value, 16);
                    }
                    catch (Exception)
                    {
                        _item.ftPayItemCase = 0;
                    }
                }
            }

            public string ftPayItemCaseData
            {
                get
                {
                    return _item.ftPayItemCaseData;
                }
                set
                {
                    _item.ftPayItemCaseData = value;
                }
            }


            [ComVisible(false)]
            public fiskaltrust.ifPOS.v0.PayItem ftPayItem()
            {
                return _item;
            }
        }

        [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
        [Guid("5D5B7FD2-3B2B-41B3-9B0C-263D495F48C3")]
        public class SignaturItem
        {
            private fiskaltrust.ifPOS.v0.SignaturItem _item;
            public SignaturItem()
            {
                _item = new fiskaltrust.ifPOS.v0.SignaturItem();
            }

            public SignaturItem(fiskaltrust.ifPOS.v0.SignaturItem item)
            {
                _item = item;
            }

            public string ftSignatureFormat
            {
                get
                {
                    return Convert.ToString(_item.ftSignatureFormat, 16);
                }
                set
                {
                    try
                    {
                        _item.ftSignatureFormat = Convert.ToInt64(value, 16);
                    }
                    catch (Exception)
                    {
                        _item.ftSignatureFormat = 0;
                    }
                }
            }

            public string ftSignatureType
            {
                get
                {
                    return Convert.ToString(_item.ftSignatureType, 16);
                }
                set
                {
                    try
                    {
                        _item.ftSignatureType = Convert.ToInt64(value, 16);
                    }
                    catch (Exception)
                    {
                        _item.ftSignatureType = 0;
                    }
                }
            }

            public string Caption
            {
                get
                {
                    return _item.Caption;
                }
                set
                {
                    _item.Caption = value;
                }
            }

            public string Data
            {
                get
                {
                    return _item.Data;
                }
                set
                {
                    _item.Data = value;
                }
            }

            [ComVisible(false)]
            public fiskaltrust.ifPOS.v0.SignaturItem ftSignaturItem()
            {
                return _item;
            }
        }

        [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
        [Guid("CCF06E1B-5A11-422B-A4C3-DA323D62F9FD")]
        public class SignResponse
        {
            private fiskaltrust.ifPOS.v0.ReceiptResponse _response;
            public SignResponse()
            {
                _response = new fiskaltrust.ifPOS.v0.ReceiptResponse();
            }

            public SignResponse(fiskaltrust.ifPOS.v0.ReceiptResponse response)
            {
                _response = response;
            }

            public string ftCashBoxID
            {
                get
                {
                    return _response.ftCashBoxID;
                }
            }

            public string cbTerminalID
            {
                get
                {
                    return _response.cbTerminalID;
                }
            }

            public string cbReceiptReference
            {
                get
                {
                    return _response.cbReceiptReference;
                }
            }

            public string ftReceiptID
            {
                get
                {
                    return "1";//_response.ftReceiptID;
                }
            }

            public DateTime ftReceiptMoment
            {
                get
                {
                    return _response.ftReceiptMoment;
                }
            }

            //public string[] ftReceiptHeader
            //{
            //    get
            //    {
            //        return _response.ftReceiptHeader;
            //    }
            //}


            //public ChargeItem[] ftChargeItems
            //{
            //    get
            //    {
            //        if (_response.ftChargeItems == null) return null;
            //        else return _response.ftChargeItems.Select(c => new ChargeItem(c)).ToArray();
            //    }
            //}

            //public string[] ftChargeLines
            //{
            //    get
            //    {
            //        return _response.ftChargeLines;
            //    }
            //}

            //public PayItem[] ftPayItems
            //{
            //    get
            //    {
            //        if (_response.ftPayItems == null) return null;
            //        else return _response.ftPayItems.Select(p => new PayItem(p)).ToArray();
            //    }
            //}

            //public string[] ftPayLines
            //{
            //    get
            //    {
            //        return _response.ftPayLines;
            //    }
            //}


            //public SignaturItem[] ftSignatures
            //{
            //    get
            //    {
            //        if (ftSignatures == null) return new SignaturItem[] { };
            //        else return _response.ftSignatures.Select(s => new SignaturItem(s)).ToArray();
            //    }
            //}

            public int ftSignaturesLen
            {
                get
                {
                    if (_response.ftSignatures == null) return 0;
                    else return _response.ftSignatures.Length;
                }
            }
            public SignaturItem GetftSignature(int index)
            {
                return new SignaturItem(_response.ftSignatures[index]);
            }

            public string[] ftReceiptFooter
            {
                get
                {
                    return _response.ftReceiptFooter;
                }
            }

            public string ftState
            {
                get
                {
                    return Convert.ToString(_response.ftState, 16);
                }
            }

            public string ftStateData
            {
                get
                {
                    return _response.ftStateData;
                }
            }

        }

        public string journal(string ftJournalType, string from, string to)
        {
            if (!Connected) throw new Exception("Not Connected");
            try
            {
                long lftJournalType, lfrom, lto;
                if (Int64.TryParse(ftJournalType, NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lftJournalType))
                {
                    if (!Int64.TryParse(from, NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lfrom)) { lfrom = 0; }
                    if (!Int64.TryParse(to, NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lto)) { lto = 0; }
                    StreamReader reader = new StreamReader(proxy.Journal(lftJournalType, lfrom , lto));
                    string text = reader.ReadToEnd();
                    return text;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        public string echo(string message)
        {
            if (!Connected) throw new Exception("Not Connected");
            try
            {
                return proxy.Echo(message);
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }




    }
}
