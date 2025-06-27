using Test.DB.Models;

namespace Test1.DB.Models
{
    public class Department
    {
        public int Id { get; set; }                       
        public int CompanyId { get; set; }                
        public string Name { get; set; } = null!;         
        public string? Phone { get; set; }                

        public Company? Company { get; set; }
    }
}
