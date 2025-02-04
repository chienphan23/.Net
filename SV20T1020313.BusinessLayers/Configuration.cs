﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020313.BusinessLayers
{   
    /// <summary>
    /// Khởi tạo, lưu trữ các thông tin cấu hình của BusinessLayers
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Chuỗi kết nối thông số kết nối đến CSDL
        /// </summary>
        public static string ConnectionString { get; private set; } = "";

        /// <summary>
        /// Khởi tạo cấu hình cho BusinessLayer
        /// Hàm này phải được gọi trước khi ứng dụng chạy
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initiallize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}
