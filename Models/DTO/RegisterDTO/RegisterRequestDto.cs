using System.ComponentModel.DataAnnotations;

namespace WebApplicationProductAPI.Models.DTO.RegisterDTO
{
    public class RegisterRequestDto
    {
        //[DataType(DataType.EmailAddress)]
        // Marks this field as an email address for the UI.
        // Helps show the right input box in forms.
        // Does not check if the email is valid.
        // Use [EmailAddress] to check if the email format is correct.

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
