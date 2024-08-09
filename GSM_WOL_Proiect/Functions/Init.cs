using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSM_WOL_Proiect
{
    public static class Init
    {
        public static void Initialize(out User[] utilizatori)
        {
            // Initialize objects from app.config
            utilizatori = ConfigHelper.GetUsers();
        }

        public static void InitializeTB(out TestBench[] testBenchuri)
        {
            // Initialize objects from app.config
            testBenchuri = ConfigHelper.Gettb();
        }
    }
}
