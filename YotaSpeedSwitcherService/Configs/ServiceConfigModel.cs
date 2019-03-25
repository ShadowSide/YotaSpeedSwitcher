using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml;

namespace YotaSpeedSwitcherService
{
    public class ServiceConfigModel
    {
        public List<ServiceAbonentModel> Abonents { get; set; }

        public static ServiceConfigModel Load()
        {
            var filePath = Path.Combine(ConfigConstants.ApplicationDataDirectory, ConfigFileName);
            if (!File.Exists(filePath))
            {
                var filePath2 = Path.Combine(ConfigConstants.ApplicationDataDirectory, ConfigFileName);
                if (!File.Exists(filePath2))
                    throw new CriticalException($"Not found service config on path '{filePath}' or '{filePath2}'");
                filePath = filePath2;
            }
            using (var stream = File.OpenRead(filePath))
            using (var textReader = new StreamReader(stream, Encoding.UTF8))
            using (var reader = XmlReader.Create(textReader))
            {
                var serializer = new DataContractJsonSerializer(typeof(ServiceConfigModel));
                return (ServiceConfigModel)serializer.ReadObject(reader);
            }
        }

        private static readonly string ConfigFileName = "ServiceConfig.json";
    }
}
