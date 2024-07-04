using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> Data { get; set; }
    }
}
