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
    public class __vars
    {
        protected string __gateway_address = "localhost";
        protected int __port_number = 24040;
        protected motPort p;

        public __vars()
        { }
    }

    [TestClass]
    public class testSockets : __vars
    {
        [TestMethod]
        public void __port_cdtor()
        {
            try
            {
                motPort __p = new motPort();
                __p.__tcp_address = __gateway_address;
                __p.__tcp_port = 80;
                __p.Open();
            }
            catch
            {
                Console.WriteLine("Failed to open port localhost:80");
            }

            try
            {
                motPort __p = new motPort();
                __p.__tcp_address = __gateway_address;
                __p.__tcp_port = 0;
                __p.Open();
            }
            catch
            {
                Console.WriteLine("Failed to open port localhost:0");
            }

            try
            {
                motPort __p = new motPort();
                __p.__tcp_address = "esmartpass.com";
                __p.__tcp_port = 80;
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
                Assert.Fail("Failed to open eSmartpass:80");
            }
        }
    }

    [TestClass]
    public class motDocTypeTests : __vars
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
                p = new motPort(__gateway_address, __port_number);
                motParser __test = new motParser(p, __xdoc, motInputStuctures.__inputXML);
            }
            catch (Exception e)
            {
                Assert.Fail("Failed XML Test - Error {0}", e.Message);              
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
                p = new motPort(__gateway_address, __port_number);
                motParser __test = new motParser(p, __jdoc, motInputStuctures.__inputJSON);
            }
            catch (Exception e)
            {
                Assert.Fail("Failed JSON Test - Error {0}", e.Message);
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
    public class recordTypeTests : __vars
    {
        [TestMethod]
        public void testDrugRecord()
        {
            motDrugRecord r;
           
            bool __failed = false;

            try
            {
                p = new motPort(__gateway_address, __port_number);
                r = new motDrugRecord("Add");
            }
            catch
            {
                Assert.Fail("Record construction failure - can't continue");
                return;
            }

            try
            {
                // Null is a valid value, should not fail
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
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("REJECTED") == false)
                {
                    Assert.Fail("Null initialization threw Exception: {0}", ex.Message);
                }
                else
                {
                    Console.WriteLine("Message from null init: {0}", ex.Message);
                }

                __failed = true;
            }           

            // Should not fail
            try
            {
                var __test_read = r.RxSys_DrugID;
                __test_read = r.LabelCode;
                __test_read = r.ProductCode;
                __test_read = r.TradeName;
                __test_read = r.Strength.ToString();
                __test_read = r.Unit;
                __test_read = r.RxOTC;
                __test_read = r.DoseForm;
                __test_read = r.Route;
                __test_read = r.DrugSchedule.ToString();
                __test_read = r.VisualDescription;
                __test_read = r.DrugName;
                __test_read = r.ShortName;
                __test_read = r.NDCNum;
                __test_read = r.SizeFactor.ToString();
                __test_read = r.Template;
                __test_read = r.DefaultIsolate.ToString();
                __test_read = r.ConsultMsg;
                __test_read = r.GenericFor;
            }
            catch(Exception ex)
            {
                Assert.Fail("Exception handler called while reading with null intializers: {0}", ex.Message);
                __failed = true;
            }

            if (!__failed)
            {
                Assert.Fail("Exception handler called while reading null intialized members");
            }

            try   // Should not fail
            {
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
            }
            catch (Exception e)
            {
                Assert.Fail("Add record action  failed with the message {0}", e.Message);
            }

            try
            {
                // Change the record slightly
                r.setField("Action", "Change");
                r.setField("TradeName", "BetaDog");
                r.setField("DrugName", "BETADOG@ 12MG");
                r.setField("ProductCode", null);
                r.setField("GenericFor", null);
                r.Write(p);
            }
            catch (Exception e)
            {
                Assert.Fail("Change action  failed with the message {0}", e.Message);
            }

            try
            {
                // Delete the Record
                r.setField("Action", "Delete");
                r.Write(p);
            }
            catch (Exception e)
            {
                Assert.Fail("Delete record action failed with the message {0}", e.Message);
            }

            __failed = false;

            try
            { 
                // Requirments (a,c,k) -- Should throw an exception
                r.setField("Action", "Add");
                r.RxSys_DrugID = "";
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
            }
            catch
            {
                __failed = true;
            }

            if (!__failed)
            {
                Assert.Fail("Exception handler not called for missing required items");
            }
        }

        [TestMethod]
        public void testStoreRecord()
        {
            motStoreRecord s;
            bool __failed = false;

            try
            {
                p = new motPort(__gateway_address, __port_number);
                s = new motStoreRecord("Add");
            }
            catch
            {
                Assert.Fail("Failed to construct Store Record -- Can't Continue");
                return;
            }

            try    // Should fail
            {
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
            }
            catch(Exception ex)
            {
                if(ex.Message.Contains("REJECTED") == false)
                {
                   Assert.Fail("Null initialization threw Exception: {0}", ex.Message); 
                }
                else
                {
                    Console.WriteLine("Message from null init: {0}", ex.Message);
                }
            }

            __failed = false;

            try // Should pass
            {
                var __test_string = s.RxSys_StoreID;
                __test_string = s.StoreName;
                __test_string = s.Address1;
                __test_string = s.Address2;
                __test_string = s.City;
                __test_string = s.State;
                __test_string = s.Zip;
                __test_string = s.Phone;
                __test_string = s.Fax;
                __test_string = s.DEANum;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Message from read null members: {0}", ex.Message);
            }

            __failed = false;

            try
            {
                var __store_id = s.RxSys_StoreID = "TEST-12345";
                var __store_name = s.StoreName = "Phreds Phabulous Pharmacy";
                var __address1 = s.Address1 = "222 Third St.";
                var __address2 = s.Address2 = "Suite 3100";
                var __city = s.City = "Cambridge";
                var __state =  s.State = "MA";
                var __zip = s.Zip = "02546";
                var __phone = s.Phone = "(617) 254-9822";
                var __fax = s.Fax = "6174251111";
                var __deanum = s.DEANum = "ABCD123456";
                s.Write(p);

                // Test the values
                if(s.RxSys_StoreID != __store_id)
                {
                    Assert.Fail("Store ID match failure");
                }

                if (s.StoreName != __store_name)
                {
                    Assert.Fail("Store Name match failure");
                }

                if (s.Address1 != __address1)
                {
                    Assert.Fail("Address1 match failure");
                }

                if (s.Address2 != __address2)
                {
                    Assert.Fail("Address2 match failure");
                }

                if (s.City != __city)
                {
                    Assert.Fail("City match failure");
                }

                if (s.State != __state)
                {
                    Assert.Fail("State match failure");
                }

                if (s.Zip != __zip)
                {
                    Assert.Fail("Zip match failure");
                }

                if (s.Phone != __phone)
                {
                    //Assert.Fail("Phone match failure");
                    Console.WriteLine("Phone number formatting? {0} : {1}", s.Phone, __phone);
                }

                if (s.Fax != __fax)
                {
                    //Assert.Fail("Fax match failure");
                    Console.WriteLine("Phone number formatting? {0} : {1}", s.Fax, __fax);
                }

                if (s.DEANum != __deanum)
                {
                    Assert.Fail("DEANum match failure");
                }

                s.StoreName = "Sally's Sensational Spot";
                s.Write(p);

                s.setField("Action", "Change");
                s.StoreName = "Brand New Fake Store";
                s.Write(p);
            }
            catch(Exception ex)
            {
                Assert.Fail("General Tests Failed: {0}", ex.Message);
                __failed = true;
            }
          

            try
            {
                s.setField("Action", "Delete");
                s.Write(p);
            }
            catch(Exception ex)
            {
                Assert.Fail("Delete record reported failure: {0}", ex.Message);
            } 
        }
    }
}
