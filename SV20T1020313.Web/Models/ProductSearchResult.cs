﻿using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; }
    }
}
