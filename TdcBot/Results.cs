﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TdcBot
{
    public class Results
    {
        public int response_code { get; set; }
        public List<Trivia> results { get; set; }
    }
}