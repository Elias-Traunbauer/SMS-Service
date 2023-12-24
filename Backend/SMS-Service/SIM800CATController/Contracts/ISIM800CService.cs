using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM800CATController.Contracts
{
    public interface ISIM800CService
    {
        public Task<bool> SendSMS(string phoneNumber, string message);
    }
}
