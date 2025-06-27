using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Company
{
    public class CompanyCreateDto
    {
        [Required(ErrorMessage = "Название обязательно.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Название должно содержать от 2 до 50 символов.")]
        public string Name { get; set; } = null!;
    }
}
