﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAspNet.Exceptions.Exceptions
{
    public class ProjectExceptionBase : SystemException
    {
        public ProjectExceptionBase(string message) : base(message) { }
    }
}
