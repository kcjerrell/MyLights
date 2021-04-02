using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights
{
    public class LightREST : IDisposable
    {
        public LightREST()
        {

        }

        static Process node;

        static LightREST()
        {

        }

        public void Dispose()
        {
            ((IDisposable)node).Dispose();
        }
    }
}
