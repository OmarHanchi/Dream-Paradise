#pragma warning disable CS8618
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BookClub.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "First Name must be more than 2 characters ")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Last Name must be more than 2 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [UniqueEmail(ErrorMessage = "This Email is already used !")]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be more than 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Relation One-to-Many
        public List<Book> BooksWritten { get; set; } = new();

        // Relation One-to-Many

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Password confirmation ")]
        [Compare("Password", ErrorMessage = "The password not matching with the one that you gave at first")]
        public string PasswordConfirm { get; set; }
    }

    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string email)
            {
                return new ValidationResult("Unvalid Email ");
            }

            var dbContext = validationContext.GetService(typeof(MyContext)) as MyContext;
            if (dbContext == null)
            {
                return new ValidationResult("Erreur interne du serveur lors de la validation de l'e-mail.");
            }

            var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                return new ValidationResult(ErrorMessage);
            }

#pragma warning disable CS8603 // Possible null reference return.
            return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
