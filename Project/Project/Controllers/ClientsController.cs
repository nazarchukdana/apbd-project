using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;
using Project.Services;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientService _service;

    public ClientsController(IClientService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ClientListDto>), 200)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetClients()
    {
        try
        {
            var clients = await _service.GetAllClientsAsync();
            return Ok(clients);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet("individuals/{id}")]
    [ProducesResponseType(typeof(IndividualResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetIndividualById(int id)
    {
        try
        {
            return Ok(await _service.GetIndividualByIdAsync(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet("companies/{id}")]
    [ProducesResponseType(typeof(CompanyResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetCompanyById(int id)
    {
        try
        {
            return Ok(await _service.GetCompanyByIdAsync(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("individuals")]
    [ProducesResponseType(typeof(IndividualResponseDto), 201)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AddIndividualClient([FromBody] CreateIndividualDto dto)
    {
        try
        {
            var client = await _service.AddIndividualClientAsync(dto);
            return CreatedAtAction(nameof(GetIndividualById), new {id = client.Id}, client);
        }
        
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("companies")]
    [ProducesResponseType(typeof(CompanyResponseDto), 201)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AddCompanyClient([FromBody] CreateCompanyDto dto)
    {
        try
        {
            var client = await _service.AddCompanyClientAsync(dto);
            return CreatedAtAction(nameof(GetCompanyById), new { id = client.Id }, client);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("individuals/{id}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(IndividualResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateIndividualClient(int id, [FromBody] UpdateIndividualDto dto)
    {
        try
        {
            var client = await _service.UpdateIndividualClientAsync(id, dto);
            return Ok(client);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [Authorize(Roles = nameof(Role.Admin))]
    [HttpPut("companies/{id}")]
    [ProducesResponseType(typeof(CompanyResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateCompanyClient(int id, [FromBody] UpdateCompanyDto dto)
    {
        try
        {
            var client = await _service.UpdateCompanyClientAsync(id, dto);
            return Ok(client);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(204)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RemoveCompanyClient(int id)
    {
        try
        {
            await _service.RemoveClientAsync(id);
            return NoContent();
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
}