using System.ComponentModel.DataAnnotations;

namespace EndpointResolver.DTOs.StudentDTOs;

public class StudentUpdateGradeDTO
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(0, int.MaxValue)]
    public int NewGrade { get; set; }
}
