using System;
using System.Collections.Generic;
using System.Text;

namespace YotaSpeedSwitcherService
{
    interface IInternetProviderService
    {
        void Login();
        IList<Fare> GetFares();
        Fare GetCurrentFare();
        void SetFare(int fareIndex);
    }
}
