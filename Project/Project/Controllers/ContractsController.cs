using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.DTOs;
using Project.Exceptions;
using Project.Services;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ContractResponseDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetContracts()
    {
        try
        {
            return Ok(await _contractService.GetContractsAsync());
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContractResponseDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetContractById(int id)
    {
        try
        {
            var contract = await _contractService.GetContractAsync(id);
            return Ok(contract);
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

    [HttpPost]
    [ProducesResponseType(typeof(ContractResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
    {
        try
        {
            var contract = await _contractService.AddContractAsync(dto);
            return CreatedAtAction(nameof(GetContractById), new {id = contract.Id }, contract);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("{contractId}/payments")]
    [ProducesResponseType(typeof(ContractResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreatePayment([FromRoute] int contractId, [FromBody] CompletePaymentDto dto)
    {
        try
        {
            var contract = await _contractService.PayForContractAsync(contractId, dto);
            return CreatedAtAction(nameof(GetContractById), new { id = contract.Id }, contract);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}