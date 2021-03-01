using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace MyFirstBot
{
    class Appearance
    {
        private SecondLife client = null;

        public Appearance(SecondLife client)
        {
            this.client = client;
        }
    }
}
