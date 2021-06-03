﻿using EmailConfigurator;
using System;

namespace EmailSmtpClientGmail
{
    public class EmailSmtpClientMS_Gmail: EmailSmtpClientMS
    {
        public EmailSmtpClientMS_Gmail()
        {
            this.Host = "smtp.gmail.com";
            this.Port = 587;
            
        }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}