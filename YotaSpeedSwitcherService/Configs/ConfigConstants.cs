using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YotaSpeedSwitcherService
{
    public static class ConfigConstants
    {
        public static string ApplicationDirectory
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string ApplicationDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            }
        }
    }
}
