using System.ComponentModel.DataAnnotations;

namespace EndpointResolver.DTOs.StudentDTOs;

public class StudentEditDTO
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Age { get; set; }
}
