using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Departament
{
    public class DepartamentDto
    {
        [Required(ErrorMessage = "Название отдела обязательно.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название отдела должно быть от 2 до 100 символов.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Телефон отдела обязателен.")]
        [Phone(ErrorMessage = "Неверный формат телефона отдела.")]
        [StringLength(20, ErrorMessage = "Телефон должен быть не длиннее 20 символов.")]
        public string Phone { get; set; } = null!;
    }
}
