﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI.Model
{
    public class Coupon
    {
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public double Discount { get; set; }
        //it can be here expire date and so on 
    }
}
