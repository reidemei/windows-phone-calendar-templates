﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.reidemeister.wp.CalendarTemplates.Resources
{
    public class ExitException : Exception
    {
        public ExitException() : base ("Exit")
        {
        }
    }
}
