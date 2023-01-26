using Application.Features.Clientes.Commands.CreateClienteCommand;
using Application.Features.Clientes.Commands.DeleteClienteCommand;
using Application.Features.Clientes.Commands.UpdateClienteCommand;
using Application.Features.Clientes.Queries.GetAllClientes;
using Application.Features.Clientes.Queries.GetClienteById;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v2;

[ApiVersion("2.0")]
public class ClienteController : BaseApiController
{
    // GET api/<controller>/
    [HttpGet()]
    public async Task<IActionResult> Get([FromQuery] GetAllClientesParameters filter)
    {
        return Ok(await Mediator.Send(new GetAllClientesQuery
        {
            PageSize = filter.PageSize,
            PageNumber = filter.PageNumber,
            Nombre = filter.Nombre,
            Apellido = filter.Apellido
        }));
    }

    // GET api/<controller>/5
    [HttpGet("{nombre}")]
    public async Task<IActionResult> Get(string nombre)
    {
        return Ok(await Mediator.Send(new GetClienteByIdQuery { Id = 1 }));
    }    
}
