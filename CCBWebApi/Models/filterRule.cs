﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class filterRule
    {
        public string field { get; set; }

        public string op { get; set; }

        public string value { get; set; }
    }
}