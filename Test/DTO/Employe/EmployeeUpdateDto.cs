using System.ComponentModel.DataAnnotations;
using Test.DTO.Departament;

namespace Test.DTO.Employe
{
    public class EmployeeUpdateDto
    {
        [Required(ErrorMessage = "Идентификатор сотрудника обязателен.")]
        [Range(1, int.MaxValue, ErrorMessage = "Идентификатор должен быть положительным числом.")]
        public int? Id { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов.")]
        public string? Name { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 50 символов.")]
        public string? Surname { get; set; }
        [Phone(ErrorMessage = "Неверный формат номера телефона.")]
        [StringLength(20, ErrorMessage = "Телефон должен содержать не более 20 символов.")]
        public string? Phone { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "CompanyId должен быть положительным числом.")]
        public int? CompanyId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId должен быть положительным числом.")]
        public int? DepartmentId { get; set; }
        public PassportUpdateDto? Passport { get; set; }

    }
}
