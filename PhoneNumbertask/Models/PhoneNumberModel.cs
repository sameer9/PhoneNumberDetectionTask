using System.ComponentModel.DataAnnotations;

namespace PhoneNumbertask.Models
{
    public class PhoneNumberModel
    {
        [Required(ErrorMessage = "Please enter a phone number.")]
        [RegularExpression(@"^(\(?\d{3}\)?-? *\d{3}-? *-?\d{4})|(\d{10})$", ErrorMessage = "Invalid phone number format.")]
        public string InputText { get; set; }

        public string ConvertedText { get; set; }

        public string ValidationResult { get; set; }
    }
}
