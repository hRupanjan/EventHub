using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Tests
{
    internal class SuperClass : AbstractClass, Interface
    {
        private int superData = 20;
        public int SuperData { get => superData; set => superData = value; }
    }
}
