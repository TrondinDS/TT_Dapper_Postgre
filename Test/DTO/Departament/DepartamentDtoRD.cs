using System.ComponentModel.DataAnnotations;

namespace Test.DTO.Departament
{
    public class DepartamentDtoRD
    {
        /// <summary>
        /// Id обязательное поле
        /// </summary>
        [Required(ErrorMessage = "Id обязателен.")]
        public int? Id { get; set; }

        /// <summary>
        /// CompanyId обязательное поле
        /// </summary>
        [Required(ErrorMessage = "CompanyId обязателен.")]
        public int? CompanyId { get; set; }

        /// <summary>
        /// Название отдела — обязательное полее
        /// </summary>
        [Required(ErrorMessage = "Название отдела обязательно.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 100 символов.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Телефон — необязательное поле, но если задан, проверяем формат
        /// </summary>
        [Phone(ErrorMessage = "Неверный формат номера телефона.")]
        [StringLength(20, ErrorMessage = "Номер телефона должен быть не длиннее 20 символов.")]
        public string? Phone { get; set; }
    }
}
