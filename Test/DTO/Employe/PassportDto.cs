using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Employe
{
    public class PassportDto
    {
        [Required(ErrorMessage = "Тип паспорта обязателен.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Тип должен содержать от 2 до 20 символов.")]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "Номер паспорта обязателен.")]
        [RegularExpression(@"^\d{6,10}$", ErrorMessage = "Номер паспорта должен содержать от 6 до 10 цифр.")]
        public string Number { get; set; } = null!;
    }
}
