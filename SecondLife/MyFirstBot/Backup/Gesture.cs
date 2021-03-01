using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace MyFirstBot
{
    class Gesture
    {
        private SecondLife client = null;
        public Gesture(SecondLife client)
        {
            this.client = client;
        }
    }
}
