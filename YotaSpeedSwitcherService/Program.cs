using System;

namespace YotaSpeedSwitcherService
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Bootstrapper();
            app.Start();
            app.Run();
        }
    }
}
