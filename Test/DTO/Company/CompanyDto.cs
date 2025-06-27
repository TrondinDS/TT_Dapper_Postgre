using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Company
{
    public class CompanyDto
    {
        [Required(ErrorMessage = "Id обязателен.")]
        [Range(1, int.MaxValue, ErrorMessage = "Id должен быть положительным числом.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Название обязательно.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Название должно содержать от 2 до 50 символов.")]
        public string Name { get; set; } = null!;
    }
}
