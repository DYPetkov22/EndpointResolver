namespace EndpointResolver.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public int Grade { get; set; }
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}