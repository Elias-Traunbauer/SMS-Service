using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM800CATController.Contracts
{
    public interface ISIM800CDiscoveryService
    {
        public Task<string> Discover();
    }
}
