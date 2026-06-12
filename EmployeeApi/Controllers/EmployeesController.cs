using EmployeeApi.DTOs;
using EmployeeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _service.GetAllAsync();
        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _service.GetByIdAsync(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateDto employeeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdEmployee = await _service.CreateAsync(employeeDto);
        return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto employeeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedEmployee = await _service.UpdateAsync(id, employeeDto);
        if (updatedEmployee == null)
            return NotFound();
        return Ok(updatedEmployee);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }
}