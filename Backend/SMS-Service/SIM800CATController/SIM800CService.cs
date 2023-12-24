using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM800CATController
{
    public class SIM800CService : Contracts.ISIM800CService
    {
        public async Task<bool> SendSMS(string phoneNumber, string message)
        {
            throw new NotImplementedException();
        }
    }
}
