using System.Globalization;

namespace SV20T1020313.Web
{
    // các lớp trong AppCodes vd SV20T1020313.Web.AppCodes => xoá đi .AppCodes => khi sử dụng sẽ khỏi using
    /// <summary>
    /// Chuyển chuỗi s sang giá trị kiểu Datetime (nếu chuyển không thành công thì trả về null)
    /// </summary>
    public static class Converters
    {
        public static DateTime? ToDateTime(this string s, string format = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            // có thêm this => mở rộng phương thức cho cái string vd ở employeeController dòng 58 . // mở rộng phương thức cho kiểu string
            // hàm extension => lớp public static => hàm public static => bổ sung this ở phía trước => extension (mở rộng phương thức cho dữ liệu kiểu chuỗi => khi . sẽ ra phương thức này)
            try
            {
                return DateTime.ParseExact(s, format.Split(";"), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
