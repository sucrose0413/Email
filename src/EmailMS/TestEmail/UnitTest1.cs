using EmailSmtpClientGmail;
using System;
using System.Net;
using System.Net.Mail;
using Xunit;

namespace TestEmail
{
    public class UnitTest1
    {
        [Fact]
        public void TestSendEmail()
        {
            
            var ms = new EmailSmtpClientMS_Gmail();
            ms.UserName = "ignat.andrei";
            //set in powershell with
            // $env:PWDGMAIL= "MyPassword"
            var pwd= Environment.GetEnvironmentVariable("PWDGMAIL");
            if (string.IsNullOrWhiteSpace(pwd))
            {
                throw new ArgumentException("please set the environment variable PWDGMAIL ;start  powershell and $env:PWDGMAIL= \"MyPassword\"");
            }
            ms.Password = Environment.GetEnvironmentVariable("PWDGMAIL");
            var client = new SmtpClient(ms.Host, ms.Port)
            {
                Credentials = new NetworkCredential(ms.UserName, ms.Password),
                EnableSsl = true
            };
            //if network security, goto https://myaccount.google.com/security, find lesser
            client.Send("ignat.andrei@gmail.com", "ignatandrei@yahoo.com", "test", "testbody");
            Console.WriteLine("Sent");
        }
    }
}
