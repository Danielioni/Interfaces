using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace motHttpListener
{/// <summary>
 /// Summary description for UnitTest1
 /// </summary>
    [TestClass]
    public class HttpListenerTests
    {
        public HttpListenerTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
       
        [TestMethod]
        public async Task SendGet()
        {
            string _site = "http://localhost:8080";
            string _query = "?test1=justtesting";

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_site);
                    client.DefaultRequestHeaders.Accept.Clear();
                    
                    //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_query);
                    Assert.AreEqual(response.Content.ReadAsStringAsync().Result, "Request Recieved");

                  
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured: " + e.Message);
            }

        }

        [TestMethod]
        public async Task SendPost()
        {
            string _site = "http://localhost:8080";
           
            var comment = "hello world";
            var senderName = "Leah";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("comment", comment),
                new KeyValuePair<string, string>("questionId", senderName)
            });


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_site);
                    client.DefaultRequestHeaders.Accept.Clear();

                    //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsync(_site, formContent);
                    Assert.AreEqual(response.Content.ReadAsStringAsync().Result, "Request Recieved");


                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured: " + e.Message);
            }

        }
    }
}
