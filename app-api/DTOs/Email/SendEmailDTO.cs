using System.ComponentModel.DataAnnotations;

public class SendEmailDTO
{
    [Required]
    public string To {get; set;} = null!;

    [Required]
    public string Body {get; set;} = null!;
}