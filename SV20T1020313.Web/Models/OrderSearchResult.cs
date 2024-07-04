using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    
        /// <summary>
        ///  Kết quả tìm kiếm và lấy danh sách đơn hàng
        /// </summary>
        public class OrderSearchResult : BasePaginationResult
        {
            public int Status { get; set; } = 0;
            public string TimeRange { get; set; } = "";
            public List<Order> Data { get; set; } = new List<Order>();
        }
}
