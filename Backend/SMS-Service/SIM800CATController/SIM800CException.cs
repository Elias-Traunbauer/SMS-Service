﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM800CATController
{
    public class SIM800CException : Exception
    {
        public SIM800CException(string? message) : base(message)
        {
        }
    }
}
