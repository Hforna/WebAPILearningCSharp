﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAspNet.Exceptions.Exceptions
{
    public class RegisterProductError : ProjectExceptionBase
    {
        public List<string> Errors { get; set; }

        public RegisterProductError(List<string> errors) : base(string.Empty)
        {
            Errors = errors;
        }

        public RegisterProductError(string error) : base(string.Empty)
        {
            Errors = new List<string>();
            Errors.Add(error);
        }

        public override IList<string> GetErrorMessages() => Errors;

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
