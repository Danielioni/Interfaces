using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using System.Net;
using NLog;
using System.Net.Mail;

namespace motCommonLib
{
   
    public class motEmail
    {
        public void __test()
        {
            string to = "jane@contoso.com";
            string from = "ben@contoso.com";
            string subject = "Using the new SMTP client.";
            string body = @"Using this new feature, you can send an e-mail message from an application very easily.";

            MailMessage message = new MailMessage(from, to, subject, body);
            SmtpClient client = new SmtpClient("smtp.google.com");
            Console.WriteLine("Changing time out from {0} to 100.", client.Timeout);

            client.Timeout = 100;

            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.

            client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
                            ex.ToString());
            }
        }
    }
}
