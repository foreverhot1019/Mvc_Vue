using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TMI.Web
{
    public class MailServerSettings : ConfigurationSection
    {
        private static MailServerSettings settings
          = ConfigurationManager.GetSection("MailServerSettings") as MailServerSettings;

        public static MailServerSettings Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("Server"
          , DefaultValue = "127.0.0.1"
          , IsRequired = true)]

        public string Server
        {
            get { return (string)this["Server"]; }
            set { this["Server"] = value; }
        }


        [ConfigurationProperty("Port"
            , DefaultValue = 25
          , IsRequired = true)]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("User"
          , DefaultValue = "admin"
          , IsRequired = false)]

        public string User
        {
            get { return (string)this["User"]; }
            set { this["User"] = value; }
        }
        [ConfigurationProperty("User"
          , DefaultValue = "admin"
          , IsRequired = false)]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

    }
}