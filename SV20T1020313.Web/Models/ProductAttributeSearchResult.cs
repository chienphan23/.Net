using Microsoft.AspNetCore.Mvc;
using SV20T1020313.DomainModels;
using System.Collections.Generic;

namespace SV20T1020313.Web.Models
{
    public class ProductAttributeSearchResult
    {
        public Product data { get; set; }
        public List<ProductPhoto>? ListDataPhoto { get; set; }
        public List<ProductAttribute>? ListDataAttribute { get; set; }
    }
}
