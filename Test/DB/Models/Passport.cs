namespace Test1.DB.Models
{
    public class Passport
    {
        public int EmployeeId { get; set; }               
        public string? Type { get; set; }
        public string? Number { get; set; }       

        public Employee? Employee { get; set; }
    }
}
