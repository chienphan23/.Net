using SV20T1020313.DomainModels;

namespace SV20T1020313.Web.Models
{
    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; }
    }
}
