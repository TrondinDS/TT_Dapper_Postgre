using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Departament
{
    public class DepartamentCreateDto
    {
        /// <summary>
        /// Идентификатор компании обязателен
        /// </summary>
        [Required(ErrorMessage = "CompanyId обязателен.")]
        [Range(1, int.MaxValue, ErrorMessage = "CompanyId должен быть положительным числом.")]
        public int CompanyId { get; set; }

        /// <summary>
        /// Название отдела обязательное поле
        /// </summary>
        [Required(ErrorMessage = "Название отдела обязательно.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно содержать от 2 до 100 символов.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Номер телефона необязательный, но если задан, должен быть валиден
        /// </summary>
        [Phone(ErrorMessage = "Неверный формат номера телефона.")]
        [StringLength(20, ErrorMessage = "Номер телефона должен быть не длиннее 20 символов.")]
        public string? Phone { get; set; }
    }
}
