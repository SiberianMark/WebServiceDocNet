using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Linq;
using System.Text;

namespace Esquel.Library.Infrastructure.Configuration
{
    public class WebConfigApplicationSettings : IApplicationSettings
    {

        public string Database
        {
            get { return ConfigurationManager.AppSettings["Database"]; }
        }

        public string LoggerName
        {
            get { return ConfigurationManager.AppSettings["LoggerName"]; }
        }

        public int Interval
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]); }
        }
    }

}
