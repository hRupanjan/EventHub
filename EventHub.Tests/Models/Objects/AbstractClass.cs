using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubProject.Tests
{
    internal abstract class AbstractClass
    {
        private int primaryData = 30;
        public int PrimaryData { get => primaryData; set => primaryData = value; }
    }
}
