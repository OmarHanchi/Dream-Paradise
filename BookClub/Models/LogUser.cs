#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace BookClub.Models;


public class LogUser
{
    [EmailAddress]
    [Required (ErrorMessage ="The Email field is required")]
    public string LogEmail { get; set; }
    [Required (ErrorMessage ="The Password field is required")]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public string LogPassword { get; set; }

}