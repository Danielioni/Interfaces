﻿// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney, Leah Bialic and Medicine-On-Time, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using NLog;


namespace motInboundLib
{
    public delegate void MotHttpOutputChangeEventHandler(Object __sender, motHttpEventArgs __args);

    public class motHttpEventArgs : EventArgs
    {
        public String _outputMessage = string.Empty;
        public DateTime _timeStamp { get; set; }

        public motHttpEventArgs(string outputMessage, DateTime timeStamp)
        {
            this._outputMessage = outputMessage;
            this._timeStamp = timeStamp;
        }

        public string OutPutMessage
        {
            get { return _outputMessage; }
            set { this._outputMessage = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { this._timeStamp = value; }
        }
    }

    public class motHttpListener
    {


        private Logger _logger;
        public HttpListener _listener;
        public event MotHttpOutputChangeEventHandler OutputTextChanged;
        private string _outputText;
        private string _fullHttpAddress;
        private string _responseString = string.Empty;
        private string _requestString = string.Empty;
        private Thread _listenThread1;
        private bool _isListening;
        

        public void StartListener(string fullHttpAddress)
        {
            _fullHttpAddress = fullHttpAddress;
           
            try
            {
                _listener = new HttpListener();
                Console.WriteLine("StartListener: {0}", Thread.CurrentThread.ManagedThreadId);
                _listener.Prefixes.Add(_fullHttpAddress);
              
                _listener.Start();
                _isListening = true;
                _outputText = "Listening for http request...";
                OnOutputTextChanged(new motHttpEventArgs(_outputText, DateTime.Now));
                Console.WriteLine(_outputText);
                //the following lines provide a way to keep the httplistener open in order to process multiple calls
                _listenThread1 = new Thread(new ParameterizedThreadStart(ListenerStart));
                _listenThread1.Start();
               
            }
            catch (Exception e)
            {
                _outputText = string.Format("An error occurred while attempting to start the Http listener: {0}\nExiting ...", e.Message);
                Console.WriteLine(_outputText);
                _logger.Error(_outputText);
                OnOutputTextChanged(new motHttpEventArgs(_outputText, DateTime.Now));
            }
           
        }

        private void ListenerStart(object s)
        {
            while (_isListening)
            {
                //this allows the listener to stay open after processing a request
                ProcessRequest();
            }
           
        
        }

        private void ProcessRequest()
        {
            IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
            Console.WriteLine("Waiting for request to be processed asyncronously.");
            result.AsyncWaitHandle.WaitOne();
            
           
        }

        public  void ListenerCallback(IAsyncResult result)
        {
            try { 
            if (_isListening == true)
                {
                    var data_text = string.Empty;
                    HttpListener listener = (HttpListener)result.AsyncState;

                    // Call EndGetContext to complete the asynchronous operation.
                    HttpListenerContext context = listener.EndGetContext(result);
                    HttpListenerRequest request = context.Request;


                    //TODO this switch block is temporary - more parsing needed
                    switch (request.HttpMethod)
                    {
                        case "GET":
                            _requestString = request.RawUrl;
                            break;
                        case "POST":
                            _requestString = new System.IO.StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
                            break;
                        default:
                            throw new HttpListenerException();
                    }
                   

                    OnOutputTextChanged(new motHttpEventArgs(_requestString, DateTime.Now));

                    //TODO - need to validate input and convert to JSON for sending to mot via REST interface


                    // Construct a response.
                    HttpListenerResponse response = context.Response;
                    response.StatusCode = 200;
                    response.StatusDescription = "OK";
                    _responseString = "Request Recieved";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(_responseString);
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                
                    OnOutputTextChanged(new motHttpEventArgs(_responseString, DateTime.Now));
                }
            }
            catch (HttpListenerException e)
            {

                _outputText = string.Format("Invalid HttpMethod : {0}", e.Message);
                Console.WriteLine(_outputText);
                _logger.Error(_outputText);
                OnOutputTextChanged(new motHttpEventArgs(_outputText, DateTime.Now));
            }


        }

        public void StopListener()
        {
            try
            {
                _isListening = false;
                _listener.Stop();
                _outputText = "Listener has been stopped...";
                OnOutputTextChanged(new motHttpEventArgs(_outputText, DateTime.Now));
                Console.WriteLine(_outputText);
            }
            catch (Exception e)
            {

                _outputText = string.Format("Errors occurred when stopping listener : {0} ", e.Message);
                Console.WriteLine(_outputText);
                _logger.Error(_outputText);
                OnOutputTextChanged(new motHttpEventArgs(_outputText, DateTime.Now));
            }
            
        }

        protected virtual void OnOutputTextChanged(motHttpEventArgs e)
        {
            if(OutputTextChanged != null)
            {
                OutputTextChanged(this, e);
            }

        }
    }
}



/// <summary>
/// 
/// Note:  A template for a RESTful/JSON Interface
///
/// 
/// </summary>

//namespace motInboundLib
//{
//    using motCommonLib;

//    public class httpInputSource
//    {
//        protected static async Task<dynamic> getJsonRecord(string __site, string __query)
//        {
//            try
//            {
//                using (var client = new HttpClient())
//                {
//                    client.BaseAddress = new Uri(__site);
//                    client.DefaultRequestHeaders.Accept.Clear();
//                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                    var response = await client.GetAsync(__query);

//                    dynamic data = null;
//                    if (response != null)
//                    {
//                        string json = response.Content.ReadAsStringAsync().Result;
//                        data = JsonConvert.DeserializeObject(json);
//                    }

//                    return data;
//                }
//            }
//            catch(Exception e)
//            {
//                throw new Exception("Failed reading REST/JSON record: " + e.Message);
//            }
//        }

//        protected static async Task<dynamic> getXmlRecord(string __site, string __query)
//        {
//            throw new NotImplementedException();

//            try
//            {
//                using (var client = new HttpClient())
//                {
//                    client.BaseAddress = new Uri(__site);
//                    client.DefaultRequestHeaders.Accept.Clear();
//                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

//                    var response = await client.GetAsync(__query);

//                    dynamic data = null;
//                    if (response != null)
//                    {
//                        //string json = response.Content.ReadAsStringAsync().Result;
//                        //data = JsonConvert.DeserializeObject(json);
//                    }

//                    return data;
//                }

//                return null;
//            }
//            catch(Exception e)
//            {
//                throw new Exception("Faild reading REST/XML record: " + e.Message);
//            }
//        }

//        public virtual motDrugRecord getDrugRecord()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual motLocationRecord getLocationRecord()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual motPatientRecord getPatientRecord()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual motPrescriptionRecord getPrescriptionRecord()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual motPrescriberRecord getPrescriberRecord()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual motStoreRecord getStoreRecord()
//        {
//            throw new NotImplementedException();
//        }
//        public virtual motTimeQtysRecord getTimeQtyRecord()
//        {
//            throw new NotImplementedException();
//        }
//        private async Task<bool> UrlExists(string url)
//        {
//            var client = new HttpClient();
//            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Head, url);
//            var response = await client.SendAsync(httpRequestMsg);

//            return (response.IsSuccessStatusCode);
//        }

//        public httpInputSource(){ }

//        ~httpInputSource() { }
//    }

//    class TestHttpInput : httpInputSource
//    {
//        private string __siteRoot = "";

//        public override motDrugRecord getDrugRecord()
//        {
//            motDrugRecord __drug = new motDrugRecord("Add");
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                __xTable.Add("Id", "RxSys_DrugID");
//                __xTable.Add("ManufacturerId", "LblCode");
//                __xTable.Add("ReOrderId", "ProdCode");
//                __xTable.Add("TradeName", "TradeName");
//                __xTable.Add("Strength", "Strength");
//                __xTable.Add("Unit", "Unit");
//                __xTable.Add("RxOtc", "RxOtc");
//                __xTable.Add("DoseForm", "DoseForm");
//                __xTable.Add("Route", "Route");
//                __xTable.Add("Schedule", "DrugSchedule");
//                __xTable.Add("FullVisualDescription", "VisualDescription");
//                __xTable.Add("RxLabelName", "DrugName");
//                __xTable.Add("CardVisualDescription", "ShortName");
//                __xTable.Add("NdcNumber", "NDCNum");
//                __xTable.Add("DrugCupCountId", "SizeFactor");
//                __xTable.Add("PackageTemplate", "Template");
//                __xTable.Add("Version", "ConsultMsg");
//                __xTable.Add("GenericForId", "GenericFor");


//                var __record = getJsonRecord(__siteRoot, @"api/drug/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Drug data " + e.Message);
//            }

//            return base.getDrugRecord();
//        }

//        public override motLocationRecord getLocationRecord()
//        {
//            motLocationRecord __location = new motLocationRecord();
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                // Load the translaton table -- Database Column Name to Gateway Tag Name                
//                __xTable.Add("", "RxSys_LocID");
//                __xTable.Add("", "RxSys_StoreID");
//                __xTable.Add("", "LocationName");
//                __xTable.Add("", "Address1");
//                __xTable.Add("", "Address2");
//                __xTable.Add("", "City");
//                __xTable.Add("", "State");
//                __xTable.Add("", "Zip");
//                __xTable.Add("", "Phone");
//                __xTable.Add("", "Comments");
//                __xTable.Add("", "CycleDays");
//                __xTable.Add("", "CycleType");

//                var __record = getJsonRecord(__siteRoot, @"api/location/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Location data " + e.Message);
//            }

//            return base.getLocationRecord();
//        }

//        public override motPatientRecord getPatientRecord()
//        {
//            motPatientRecord __patient = new motPatientRecord();
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                // Load the translaton table -- Database Column Name to Gateway Tag Name  
//                __xTable.Add("", "RxSys_PatID");
//                __xTable.Add("", "LastName");
//                __xTable.Add("", "FirstName");
//                __xTable.Add("", "MiddleInitial");
//                __xTable.Add("", "Address1");
//                __xTable.Add("", "Address2");
//                __xTable.Add("", "City");
//                __xTable.Add("", "State");
//                __xTable.Add("", "Zip");
//                __xTable.Add("", "Phone1");
//                __xTable.Add("", "Phone2");
//                __xTable.Add("", "WorkPhone");
//                __xTable.Add("", "RxSys_LocID");
//                __xTable.Add("", "Room");
//                __xTable.Add("", "Comments");
//                __xTable.Add("", "CycleDate");
//                __xTable.Add("", "CycleDays");
//                __xTable.Add("", "CycleType");
//                __xTable.Add("", "Status");
//                __xTable.Add("", "RxSys_LastDoc");
//                __xTable.Add("", "RxSys_PrimaryDoc");
//                __xTable.Add("", "RxSys_AltDoc");
//                __xTable.Add("", "SSN");
//                __xTable.Add("", "Allergies");
//                __xTable.Add("", "Diet");
//                __xTable.Add("", "DxNotes");
//                __xTable.Add("", "TreatmentNotes");
//                __xTable.Add("", "DOB");
//                __xTable.Add("", "Height");
//                __xTable.Add("", "Weight");
//                __xTable.Add("", "ResponsibleName");
//                __xTable.Add("", "InsName");
//                __xTable.Add("", "InsPNo");
//                __xTable.Add("", "AltInsName");
//                __xTable.Add("", "AltInsPNo");
//                __xTable.Add("", "MCareNum");
//                __xTable.Add("", "MCaidNum");
//                __xTable.Add("", "AdmitDate");
//                __xTable.Add("", "ChartOnly");
//                var __record = getJsonRecord(__siteRoot, @"api/Patient/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Patient data " + e.Message);
//            }
//            return base.getPatientRecord();
//        }

//        public override motPrescriberRecord getPrescriberRecord()
//        {
//            motPrescriberRecord __prescriber = new motPrescriberRecord();
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                __xTable.Add("", "RxSys_DocID");
//                __xTable.Add("", "LastName");
//                __xTable.Add("", "FirstName");
//                __xTable.Add("", "Address1");
//                __xTable.Add("", "Address2");
//                __xTable.Add("", "City");
//                __xTable.Add("", "Zip");
//                __xTable.Add("", "Phone");
//                __xTable.Add("", "Comments");
//                __xTable.Add("", "DEA_ID");
//                __xTable.Add("", "TPID");
//                __xTable.Add("", "Speciality");

//                var __record = getJsonRecord(__siteRoot, @"api/Prescriber/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Prescriber data " + e.Message);
//            }
//            return base.getPrescriberRecord();
//        }

//        public override motPrescriptionRecord getPrescriptionRecord()
//        {
//            motPrescriptionRecord __scrip = new motPrescriptionRecord();
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                // Load the translaton table -- Database Column Name to Gateway Tag Name                
//                __xTable.Add("", "RxSys_RxNum");
//                __xTable.Add("", "RxSys_PatID");
//                __xTable.Add("", "RxSys_DocID");
//                __xTable.Add("", "RxSys_DrugID");
//                __xTable.Add("", "Sig");
//                __xTable.Add("", "RxStartDate");
//                __xTable.Add("", "RxStopDate");
//                __xTable.Add("", "DiscontinueDate");
//                __xTable.Add("", "DoseScheduleName");
//                __xTable.Add("", "Comments");
//                __xTable.Add("", "Refills");
//                __xTable.Add("", "RxSys_NewRxNum");
//                __xTable.Add("", "Isolate");
//                __xTable.Add("", "RxType");
//                __xTable.Add("", "MDOMStart");
//                __xTable.Add("", "MDOMEnd");
//                __xTable.Add("", "QtyPerDose");
//                __xTable.Add("", "QtyDispensed");
//                __xTable.Add("", "Status");
//                __xTable.Add("", "DoW");
//                __xTable.Add("", "SpecialDoses");
//                __xTable.Add("", "DoseTimeQtys");
//                __xTable.Add("", "ChartOnly");
//                __xTable.Add("", "AnchorDate");

//                var __record = getJsonRecord(__siteRoot, @"api/Prescription/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Prescription data " + e.Message);
//            }

//            return base.getPrescriptionRecord();
//        }

//        public override motStoreRecord getStoreRecord()
//        {
//            motStoreRecord __store = new motStoreRecord();
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                // Load the translaton table -- Database Column Name to Gateway Tag Name                
//                __xTable.Add("", "RxSys_StoreID");
//                __xTable.Add("", "StoreName");
//                __xTable.Add("", "Address1");
//                __xTable.Add("", "Address2");
//                __xTable.Add("", "City");
//                __xTable.Add("", "State");
//                __xTable.Add("", "Zip");
//                __xTable.Add("", "Phone");
//                __xTable.Add("", "Fax");
//                __xTable.Add("", "DEANum");

//                var __record = getJsonRecord(__siteRoot, @"api/store/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Store data: " + e.Message);
//            }

//            return base.getStoreRecord();
//        }

//        public override motTimeQtysRecord getTimeQtyRecord()
//        {
//            motTimeQtysRecord __tq = new motTimeQtysRecord();
//            Dictionary<string, string> __xTable = new Dictionary<string, string>();

//            try
//            {
//                // Load the translaton table -- Database Column Name to Gateway Tag Name                
//                __xTable.Add("", "RxSys_LocID");
//                __xTable.Add("", "DoseScheduleName");
//                __xTable.Add("", "DoseTimeQtys");

//                var __record = getJsonRecord(__siteRoot, @"api/TQ/1");

//                if (__record != null)
//                {
//                    // Got something, now transform it to what we need
//                    // SourceRecord s = <SourceRecord> __record;
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to get Time/Quantity data: " + e.Message);
//            }

//            return base.getTimeQtyRecord();
//        }

//        TestHttpInput(string __rootPath)
//        {
//            __siteRoot = __rootPath;
//        }

//    }
//}