using System;
using System.Collections.Generic;
using System.Text;

namespace YotaSpeedSwitcherService
{
    public class ServiceAbonentModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int MinimalSpeed { get; set; }
        public int MaximalSpeed { get; set; }
        public int MaximalPrice { get; set; }
        public bool Enabled { get; set; }

        public ProviderAuthorization ProviderAuthorization { get { return new ProviderAuthorization(Login, Password); } }
    }
}
