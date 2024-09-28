﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DAL.DTO.Req
{
    public class ReqUpdateBalanceDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "balance must be a positive value")]
        public decimal balance {  get; set; }
    }
}
