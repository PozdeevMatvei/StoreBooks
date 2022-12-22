﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Web.App.Models
{
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = "";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
