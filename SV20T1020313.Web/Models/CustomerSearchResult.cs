using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    /// <summary>
    ///  Kết quả tìm kiếm và lấy danh sách khách hàng
    /// </summary>
    public class CustomerSearchResult : BasePaginationResult
    {
        public List<Customer> Data { get; set; }
    }
}
