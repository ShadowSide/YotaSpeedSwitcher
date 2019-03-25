using System;
using System.Collections.Generic;
using System.Text;

namespace YotaSpeedSwitcherService
{
    public class Fare
    {
        public Fare (int index, int price, int speed)
        {
            Index = index;
            Price = price;
            Speed = speed;
        }
        public int Index { get; private set; }
        public int Price { get; private set; }
        public int Speed { get; private set; }
    }
}
