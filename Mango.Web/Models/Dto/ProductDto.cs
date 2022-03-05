﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Models.Dto
{
    public class ProductDto
    {

        public ProductDto()
        {
            Count = 1;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        [Range(0, 100)]
        public int Count { get; set; }
    }
}