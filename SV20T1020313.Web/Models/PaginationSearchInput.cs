namespace SV20T1020313.Web.Models
{
    /// <summary>
    /// Lưu trữ đầu vào tìm kiếm , đầu vào tìm kiếm dữ liệu để nhận dữ liệu dưới dạng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }

    /// <summary>
    /// Đầu vào sử dụng cho tìm kiếm mặt hàng
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput 
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;

    }
    public class OrderSearchInput : PaginationSearchInput
    {
        public int Status { get; set; } = 0;
        public string DateRange { get; set; } = "";
        public DateTime? FromTime 
        { 
            get
            {
                if(string.IsNullOrWhiteSpace(DateRange))
                    return null;
                string[] times = DateRange.Split('-');
                if(times.Length == 2 )
                {
                    DateTime? value = Converters.ToDateTime(times[0].Trim());
                    return value;
                }
                return null;
            } 
        }
        public DateTime? ToTime
        {
            get
            {
                if(string.IsNullOrWhiteSpace(DateRange))
                    return null;
                string[] times = DateRange.Split('-');
                if(times.Length == 2)
                {
                    DateTime? value = Converters.ToDateTime(times[1].Trim());
                    if (value.HasValue)
                    {
                        value = value.Value.AddMilliseconds(86399998);
                    }
                    return value;
                }
                return null;
            }
        }

    }
}
