using System;
using System.IO;
using System.Security.Permissions;

namespace motInboundLib
{
    class motMain
    {
        static void testXMLDoc()
        {
            string __xdoc = @"<?xml version=\""1.0\"" encoding=\""utf-8\"" ?>
                              <Record xmlns=\""http://fred.medicineontime.com/mot-records/drug.xsd\"">
                                <Table>Drug</Table>
                                <Action>Add</Action>
                                <RxSys_DrugID required=\\""\""true\\""\"" size=\\""\""11\\""\"">AS2145</RxSys_DrugID> <!-- Unique identifier for the drug in the RxSystem - REQUIRED -->
                                <LblCode size=\""6\"">QQQ1W</LblCode>            <!-- FDA Labeler Code -->
                                <ProdCode size=\""4\"">9876</ProdCode>          <!-- FDA Product Code -->
                                <TradeName size=\""100\"">Tylenol</TradeName>      <!-- Trade name for the drug -->
                                <Strength size=\""10\"">500</Strength>         <!-- Single dose strength value -->
                                <Unit size=\""10\"">mg</Unit>                 <!-- Single dose strength units (mg, etc ..) -->
                                <RxOTC size=\""1\"">O</RxOTC>                <!-- R or O with O meaning available over the counter -->    
                                <DoseForm size=\""11\"">Tablet</DoseForm>         <!-- Tablet/Capsule/Inhaler/... -->    
                                <Route size=\""9\"">Oral</Route>                <!-- Oral/Nasel/IV/Injection/... -->
                                <DrugSchedule maxvalue=\""7\"">2</DrugSchedule>  <!-- FDA Drug Schedule (identifier) -->
                                <VisualDescription size=\""12\"">Oval/White</VisualDescription> <!-- Physical description -->
                                <DrugName required=\""true\"" size=\""40\"">Robert</DrugName>   <!-- Name for MOT Picklist (40 bytes of tradename used if left blank -->
                                <ShortName size=\""16\"">Bob</ShortName>       <!-- Name that appears on MOT card labels (16 bytes of drugname used if left blank -->
                                <NDCNum required=\""true\"" size=\""11\"">0000123412</NDCNum> <!-- Full NDC number withut the \""-s\"" -->
                                <SizeFactor maxvalue=\""99\"">2</SizeFactor>   <!-- Size factor relating to how many will fit in an MOT bubble (1-7), 99 = bulk drug that doesnn't get packaged -->
                                <Template size=\""1\"">B</Template>          <!-- -?> BRAD/GAIL, Unclear on the description here, can you clarify? -->
                                <DefaultIsolate size=\""1\"">0</DefaultIsolate> <!-- Binary Isolate/Not in package -->
                                <ConsultMsg size=\""45\""></ConsultMsg>     <!-- Short message, 'Don't drive over lightspeed', etc. -->
                                <GenericFor size=\""40\""></GenericFor>     <!-- Space to define sets of similar generic replacements -->
                            </Record>";

            try
            {
                Parser __test = new Parser(__xdoc, InputStucture.__inputXML);
            }
            catch(Exception e)
            {
                Console.Write("Failed XML Test - Error {0}", e.Message);
            }
        }

        static void testJSONDoc()
        {
            string __jdoc = @"{
                                \""Table\"": \""Drug\"",
                                \""Action\"": \""Add\"",
                                \""RxSys_DrugD\"": \""0000123491\"",
                                \""LblCode\"": \""00000\"",
                                \""ProdCode\"": \""0000\"",
                                \""TradeName\"": \""Tylenol\"",
                                \""Strength\"": \""500\"",
                                \""Unit\"": \""mg\"",
                                \""RxOTC\"": \""O\"",
                                \""DoseForm\"": \""Tablet\"",
                                \""Route\"": \""Oral\"",
                                \""DrugSchedule\"": 2,
                                \""VisualDescription\"": \""White /Oval\"",
                                \""DrugName\"": \""Robert\"",
                                \""ShortName\"": \""Bob\"",
                                \""NDCNum\"": \""0000123491\"",
                                \""SizeFactor\"": 2,
                                \""Template\"": \""B\"",
                                \""DefaultIsolate\"": \""0\"",
                                \""ConsultMsg\"": \""Some Text\"",
                                \""GenericFor\"": \""Some More Text\""
                              }";

            try
            {
                Parser __test = new Parser(__jdoc, InputStucture.__inputJSON);
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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        static void Run()
        {
            testXMLDoc();
            testJSONDoc();
        }

        static void Main(string[] args)
        {
            Run();
        }
    }

}