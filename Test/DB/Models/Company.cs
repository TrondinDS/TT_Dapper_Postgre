using Test1.DB.Models;

namespace Test.DB.Models
{
    public class Company
    {
        public int Id { get; set; }          
        public string Name { get; set; } = null!;  

        public List<Department> Departments { get; set; } = new();
    }
}
