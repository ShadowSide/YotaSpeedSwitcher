using System;
using System.Collections.Generic;
using System.Text;

namespace YotaSpeedSwitcherService
{
    public class ProviderAuthorization
    {
        public ProviderAuthorization (string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; private set; }
        public string Password { get; private set; }
    }
}
