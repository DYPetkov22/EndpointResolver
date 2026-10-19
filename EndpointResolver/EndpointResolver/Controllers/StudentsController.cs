using System.ComponentModel.DataAnnotations;
using EndpointResolver.Data;
using EndpointResolver.DTOs.StudentDTOs;
using EndpointResolver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace EndpointResolver.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StudentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<StudentResponseDTO>>> GetAll()
    {
        var students = await _context.Students
            .AsNoTracking()
            .Select(student => new StudentResponseDTO
            {
                Id = student.Id,
                Age = student.Age,
                Names = student.FirstName + " " + student.LastName
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet]
    public async Task<ActionResult<StudentResponseDTO>> GetOne([FromQuery, BindRequired, Range(1, int.MaxValue)] int id)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Where(student => student.Id == id)
            .Select(student => new StudentResponseDTO
            {
                Id = student.Id,
                Age = student.Age,
                Names = student.FirstName + " " + student.LastName
            })
            .FirstOrDefaultAsync();

        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentEditDTO createStudent)
    {
        var entity = new Student
        {
            FirstName = createStudent.FirstName,
            LastName = createStudent.LastName,
            Age = createStudent.Age,
            UserName = createStudent.UserName,
            Grade = 0,
            // The reference project sets a shared "TempPassword". No authentication
            // is implemented here, so do not create a hard-coded usable password.
            Password = string.Empty
        };

        _context.Students.Add(entity);
        await _context.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] StudentEditDTO updateStudent)
    {
        if (updateStudent.Id is not { } id)
            return BadRequest("Id is required for an update.");

        var entity = await _context.Students.FindAsync(id);
        if (entity is null)
            return NotFound();

        entity.FirstName = updateStudent.FirstName;
        entity.LastName = updateStudent.LastName;
        entity.Age = updateStudent.Age;
        entity.UserName = updateStudent.UserName;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery, BindRequired, Range(1, int.MaxValue)] int id)
    {
        var entity = await _context.Students.FindAsync(id);
        if (entity is null)
            return NotFound();

        _context.Students.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateGrade([FromBody] StudentUpdateGradeDTO updateStudent)
    {
        var entity = await _context.Students.FindAsync(updateStudent.StudentId);
        if (entity is null)
            return NotFound();

        entity.Grade = updateStudent.NewGrade;
        await _context.SaveChangesAsync();
        return Ok();
    }
}
