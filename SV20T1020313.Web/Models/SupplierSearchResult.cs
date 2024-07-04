using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> Data { get; set; }
    }
}
