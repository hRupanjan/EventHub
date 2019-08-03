using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Tests
{
    internal class ObjectClass : SuperClass
    {
        private int subData = 10;
        public int SubData { get => subData; set => subData = value; }
    }
}
