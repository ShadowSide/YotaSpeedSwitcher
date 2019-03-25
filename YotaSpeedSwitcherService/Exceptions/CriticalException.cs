using System;
using System.Collections.Generic;
using System.Text;

namespace YotaSpeedSwitcherService
{
    public class CriticalException: Exception
    {
        public CriticalException(string message) : base(message)
        {

        }
    }
}
