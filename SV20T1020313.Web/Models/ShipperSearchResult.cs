using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> Data { get; set; }
    }
}
