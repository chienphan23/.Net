﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020313.DomainModels
{
    /// <summary>
    /// Nhân viên
    /// </summary>
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Photo { get; set; } = "";
        public string RoleNames { get; set; } = "";
        public bool IsWorking { get; set; } = true;
    }
}
