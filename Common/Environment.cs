using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common
{
    public static class EnvironmentSettings
    {
        public static string ApplicationDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string ApplicationDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
    }
}
