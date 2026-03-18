using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    internal class cards
    {
        public int card1 { get; private set; }
        public int card2 { get; private set; }
        public int card3 { get; private set; }
        public int card4 { get; private set; }

        static public int setCard()
        {
            Random random = new Random();
            return random.Next(2, 15);
        }

    }
}

