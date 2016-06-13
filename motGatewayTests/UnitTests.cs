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
using System.Data;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Threading;

namespace motInboundLib
{
    class motMain
    {
        static void testXMLDoc()
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
                Port p = new Port("192.168.0.140", "24042");
                Parser __test = new Parser(p, __xdoc, InputStucture.__inputXML);
            }
            catch(Exception e)
            {
                Console.Write("Failed XML Test - Error {0}", e.Message);
            }
        }

        static void testJSONDoc()
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
                Port p = new Port("192.168.0.140", "24042");
                Parser __test = new Parser(p, __jdoc, InputStucture.__inputJSON);
            }
            catch (Exception e)
            {
                Console.Write("Failed JSON Test - Error {0}", e.Message);
            }
        }

        static void testTaggedDoc()
        {
        }

        static void testDelimitedDoc()
        {
        }

        static void testDrugRecord()
        {
            motDrugRecord r;
            Port p;

            try
            {
                p = new Port("127.0.0.1", "24042");
                r = new motDrugRecord("Add");

                for (int i = 0; i < 1024; i++)
                {

                    r.RxSys_DrugID = "ABDC12579";
                    r.LabelCode = "Mumble";


                    r.ProductCode = "1234";
                    r.TradeName = "ALPHAFROG@ 12 MG Tablet";
                    r.Strength = 12;
                    r.Unit = "MG";

                  

                    r.RxOTC = "R";
                    r.DoseForm = "Tablet";
                    r.Route = "Oral";
                    r.DrugSchedule = 6;

                    r.VisualDescription = "RND/RED/TAB";
                    r.DrugName = "ALPHAFROG@ 12 MG";
                    r.ShortName = "HAL 12MG";
                    r.NDCNum = "00023-0327-01";
                    r.SizeFactor = 5;
                    r.Template = "C";
                    r.DefaultIsolate = 0;
                    r.ConsultMsg = "Don't set your hair on fire";
                    r.GenericFor = "N/A";


                    r.Write(p);

                    r.setField("Action", "Change");
                    r.setField("TradeName", "BetaDog");
                    r.setField("DrugName", "BETADOG@ 12MG");

                    r.Write(p);

                    r.setField("Action", "Delete");

                    r.Write(p);
                }              
            }
            catch(Exception e)
            {
                Console.Write("testDrug Record Failure {0}", e.Message);
            }
        }

        class cprPlus : databaseInputSource
        {
           
            public cprPlus(dbType __type, string DSN) : base(__type, DSN)
            { }

            // Find all new Drug Records and add them to the system
            public override motPrescriptionRecord getPrescriptionRecord()
            {
/*
                try
                {
            
                    motPrescriberRecord __scrip = new motPrescriberRecord();
                    List<Field> __stage = new List<Field>();
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    Port p = new Port("127.0.0.1", "24042");

                    // Load the translaton table -- Database Column Name to Gateway Tag Name 
                    __xTable.Add("Id", "Id");
                    // ...

                    DataSet __recordSet = db.executeQuery("SELECT * from Rxes");

                    foreach(DataTable __table in __recordSet)
                    {
                        for(int i = 0; i < __record.FieldCount; i++)
                        {                            
                            __scrip.setField(__xTable[__record.GetName(i).ToString()],  // Column
                                                      __record.GetValue(i).ToString()); // Value
                        }

                        __scrip.Write(p);                    
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }
*/
                return base.getPrescriptionRecord();
            }
        }


       static void catch_socket_data(string __data)
        {
            Console.WriteLine("Got {0}", __data);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        static void Run()
        {
            /*
            //Test Database Connection
            try
            {
                cprPlus cpr = new cprPlus(dbType.NPGServer, "server=127.0.0.1;port=5432;userid=mot;password=mot!cool;database=Mot");
                cpr.getDrugRecord();
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to open Database for input {0}", e.Message);
            }
            */

            // Testing
            //testDrugRecord();

            // Works
            //testXMLDoc();
            //testJSONDoc();

            //fileSystemWatcher f = new fileSystemWatcher("C:\\MOT_IO");


            try
            {
                // create the listener socket on port 50005 and pass a callback function
                motSocket ms = new motSocket(21110, catch_socket_data);

                // This will start the listener and call the callback forever
                Thread __worker = new Thread(new ThreadStart(ms.listen));
                __worker.Name = "listener";
                __worker.Start();

                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(1024);
                    Console.WriteLine("Still waiting {0}", i);
                }

                ms.close();
                __worker.Join();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        static void Main(string[] args)
        {
            Run();
        }
    }

}