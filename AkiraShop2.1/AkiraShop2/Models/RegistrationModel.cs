 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop2.Models
{
    public class RegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail адрес")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пароли не совпадают!")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 2)]
        [Display(Name = "Полный адрес")]
        public string Address { get; set; }

        [Required(ErrorMessage ="Введите корректный индекс!")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Почтовый индекс")]
        [RegularExpression("^[0-9]{6}")]
        public string PostCode{ get; set; }

        [Required(ErrorMessage = "Введите корректный номер телефона!")]
        [Display(Name = "Номер телефона")]
        [RegularExpression(@"((8|\+7)-?)?\(?\d{3,5}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}((-?\d{1})?-?\d{1})?")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Соглашение с условиями пользования")]
        public bool AcceptUserAgreement { get; set; }

        //[Required]
        public string RegistrationInValid { get; set; }
    }
}
