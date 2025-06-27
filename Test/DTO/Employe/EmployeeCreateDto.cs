using System.ComponentModel.DataAnnotations;
using Test.DTO.Departament;

namespace Test.DTO.Employe
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Имя обязательно.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Фамилия обязательна.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 50 символов.")]
        public string Surname { get; set; } = null!;

        [Required(ErrorMessage = "Телефон обязателен.")]
        [Phone(ErrorMessage = "Неверный формат номера телефона.")]
        [StringLength(20, ErrorMessage = "Телефон должен содержать не более 20 символов.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "CompanyId обязателен.")]
        [Range(1, int.MaxValue, ErrorMessage = "CompanyId должен быть положительным числом.")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "DepartmentId обязателен.")]
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId должен быть положительным числом.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Паспорт обязателен.")]
        public PassportDto Passport { get; set; } = null!;

    }
}
