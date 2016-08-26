// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney and Medicine-On-Time, LLC.
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
using System.IO;
using System.Collections.Generic;
using System.Threading;
using motCommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace motRecordTests
{
    [TestClass]
    public class testSockets
    {
        [TestMethod]
        public void __port_cdtor()
        {
            try
            {
                motPort __p = new motPort();
                __p.tcp_address = "localhost";
                __p.tcp_port = 80;
                __p.Open();
            }
            catch
            {
                Console.WriteLine("Failed to open port localhost:80");
            }

            try
            {
                motPort __p = new motPort();
                __p.tcp_address = "localhost";
                __p.tcp_port = 0;
                __p.Open();
            }
            catch
            {
                Console.WriteLine("Failed to open port localhost:0");
            }

            try
            {
                motPort __p = new motPort();
                __p.tcp_address = "esmartpass.com";
                __p.tcp_port = 80;
                __p.Open();

                Console.WriteLine("Successfully opened esmartpass.com:80");
                __p.Close();

                motPort __pq = new motPort("esmartpass.com", 80);
                Console.WriteLine("Successfully opened esmartpass.com:80 using int port constructor");
                __pq.Close();

                motPort __pqr = new motPort("esmartpass.com", 80);
                Console.WriteLine("Successfully opened esmartpass.com:80 using string port constructor");
                __pqr.Close();

            }
            catch
            {
                Console.WriteLine("Failed to open port localhost:0");
            }
        }
    }

    [TestClass]
    public class motDocTypeTests
    {
        [TestMethod]
        public void testXMLDoc()
        {
            string __xdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                              <Record xmlns=""http://fred.medicineontime.com/mot-records/drug.xsd"">
                                <Table>Drug</Table>
                                <Action>Add</Action>
                                <RxSys_DrugID required=""true"" size=""11"">AS2145</RxSys_DrugID> <!-- Unique identifier for the drug in the RxSystem - REQUIRED -->
                                <LblCode size=""6"">QQQ1W</LblCode>            <!-- FDA Labeler Code -->
                                <ProdCode size=""4"">9876</ProdCode>          <!-- FDA Product Code -->
                                <TradeName size=""100"">Tylenol</TradeName>      <!-- Trade name for the drug -->
                                <Strength size=""10"">500</Strength>         <!-- Single dose strength value -->
                                <Unit size=""10"">mg</Unit>                 <!-- Single dose strength units (mg, etc ..) -->
                                <RxOTC size=""1"">O</RxOTC>                <!-- R or O with O meaning available over the counter -->    
                                <DoseForm size=""11"">Tablet</DoseForm>         <!-- Tablet/Capsule/Inhaler/... -->    
                                <Route size=""9"">Oral</Route>                <!-- Oral/Nasel/IV/Injection/... -->
                                <DrugSchedule maxvalue=""7"">2</DrugSchedule>  <!-- FDA Drug Schedule (identifier) -->
                                <VisualDescription size=""12"">Oval/White</VisualDescription> <!-- Physical description -->
                                <DrugName required=""true"" size=""40"">Robert</DrugName>   <!-- Name for MOT Picklist (40 bytes of tradename used if left blank -->
                                <ShortName size=""16"">Bob</ShortName>       <!-- Name that appears on MOT card labels (16 bytes of drugname used if left blank -->
                                <NDCNum required=""true"" size=""11"">0000123412</NDCNum> <!-- Full NDC number withut the ""-s"" -->
                                <SizeFactor maxvalue=""99"">2</SizeFactor>   <!-- Size factor relating to how many will fit in an MOT bubble (1-7), 99 = bulk drug that doesnn't get packaged -->
                                <Template size=""1"">B</Template>          <!-- -?> BRAD/GAIL, Unclear on the description here, can you clarify? -->
                                <DefaultIsolate size=""1"">0</DefaultIsolate> <!-- Binary Isolate/Not in package -->
                                <ConsultMsg size=""45""></ConsultMsg>     <!-- Short message, 'Don't drive over lightspeed', etc. -->
                                <GenericFor size=""40""></GenericFor>     <!-- Space to define sets of similar generic replacements -->
                            </Record>";

            try
            {
                motPort p = new motPort("localhost", "24042");
                motParser __test = new motParser(p, __xdoc, motInputStuctures.__inputXML);
            }
            catch (Exception e)
            {
                Console.Write("Failed XML Test - Error {0}", e.Message);
            }
        }

        [TestMethod]
        public  void testJSONDoc()
        {
            string __jdoc = @"{
                                ""Table"": ""Drug"",
                                ""Action"": ""Add"",
                                ""RxSys_DrugID"": ""0000123491"",
                                ""LblCode"": ""00000"",
                                ""ProdCode"": ""0000"",
                                ""TradeName"": ""Tylenol"",
                                ""Strength"": ""500"",
                                ""Unit"": ""mg"",
                                ""RxOTC"": ""O"",
                                ""DoseForm"": ""Tablet"",
                                ""Route"": ""Oral"",
                                ""DrugSchedule"": 2,
                                ""VisualDescription"": ""White /Oval"",
                                ""DrugName"": ""Robert"",
                                ""ShortName"": ""Bob"",
                                ""NDCNum"": ""0000123491"",
                                ""SizeFactor"": 2,
                                ""Template"": ""B"",
                                ""DefaultIsolate"": ""0"",
                                ""ConsultMsg"": ""Some Text"",
                                ""GenericFor"": ""Some More Text""
                              }";

            try
            {
                motPort p = new motPort("localhost", "24042");
                motParser __test = new motParser(p, __jdoc, motInputStuctures.__inputJSON);
            }
            catch (Exception e)
            {
                Console.Write("Failed JSON Test - Error {0}", e.Message);
            }
        }

        [TestMethod]
        public void testTaggedDoc()
        {
        }

        [TestMethod]
        public void testDelimitedDoc()
        {
        }
    }

    [TestClass]
    public class recordTypeTests
    {
        [TestMethod]
        public void testDrugRecord()
        {
            motDrugRecord r;
            motPort p;

            try
            {
                p = new motPort("localhost", "24041");
                r = new motDrugRecord("Add");

                // Null is a valid value
                r.RxSys_DrugID = null;
                r.LabelCode = null;
                r.ProductCode = null;
                r.TradeName = null;
                r.Strength = 0;
                r.Unit = null;
                r.RxOTC = null;
                r.DoseForm = null;
                r.Route = null;
                r.DrugSchedule = 0;
                r.VisualDescription = null;
                r.DrugName = null;
                r.ShortName = null;
                r.NDCNum = null;
                r.SizeFactor = 0;
                r.Template = null;
                r.DefaultIsolate = 0;
                r.ConsultMsg = null;
                r.GenericFor = null;
                r.Write(p);

                // Now use some actual values
                r.RxSys_DrugID = "ABDC12579";
                r.LabelCode = "Mumble";
                r.ProductCode = "1234";
                r.TradeName = "ALPHAFROG@ 120 MG Tablet";
                r.Strength = 12;
                r.Unit = "MG";
                r.RxOTC = "R";
                r.DoseForm = "Tablet";
                r.Route = "Oral";
                r.DrugSchedule = 6;
                r.VisualDescription = "RND/RED/TAB";
                r.DrugName = "ALPHAFROG@ 120 MG";
                r.ShortName = "HAL 120MG";
                r.NDCNum = "00023-0337-01";
                r.SizeFactor = 5;
                r.Template = "C";
                r.DefaultIsolate = 0;
                r.ConsultMsg = "Don't set your hair on fire";
                r.GenericFor = "N/A";
                r.Write(p);

                // Change the record slightly
                r.setField("Action", "Change");
                r.setField("TradeName", "BetaDog");
                r.setField("DrugName", "BETADOG@ 12MG");
                r.setField("ProductCode", null);
                r.setField("GenericFor", null);
                r.Write(p);

                // Delete the ecord
                r.setField("Action", "Delete");
                r.Write(p);
            }
            catch (Exception e)
            {
                Console.Write("testDrug Record Failure {0}", e.Message);
            }
        }

        [TestMethod]
        public void testStoreRecord()
        {
            motStoreRecord s;
            motPort p;

            try
            {
                p = new motPort("localhost", "24041");
                s = new motStoreRecord("Add");

                s.RxSys_StoreID = null;
                s.StoreName = null;
                s.Address1 = null;
                s.Address2 = null;
                s.City = null;
                s.State = null;
                s.Zip = null;
                s.Phone = null;
                s.Fax = null;
                s.DEANum = null;
                s.Write(p);

                s.RxSys_StoreID = "ABCD123456";
                s.StoreName = "Phreds Phabulous Pharmacy";
                s.Address1 = "222 Third St.";
                s.Address2 = "Suite 3100";
                s.City = "Cambridge";
                s.State = "ma";
                s.Zip = "02546";
                s.Phone = "(617) 254-9822";
                s.Fax = "6174251111";
                s.DEANum = "ABCD123456";
                s.Write(p);

                s.StoreName = "Sally's Sensational Spot";
                s.Write(p);
            }
            catch
            {
                throw;
            }
        }
    }
}
