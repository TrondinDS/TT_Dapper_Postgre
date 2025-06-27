using Test.DB.Models;

namespace Test1.DB.Models
{
    public class Employee
    {
        public int Id { get; set; }                       
        public string? Name { get; set; }         
        public string? Surname { get; set; }    
        public string? Phone { get; set; }                

        public int? CompanyId { get; set; }                
        public int? DepartmentId { get; set; }             

        public Company? Company { get; set; }             
        public Department? Department { get; set; }       
        public Passport? Passport { get; set; }
    }
}
