using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sprint4.DTO.Yard;
using sprint4.HATEOAS;
using sprint4.Services;

namespace sprint4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YardController : Controller
{
    private readonly IService<YardResponse, YardDTO> _service;
    private readonly LinkGenerator _linkGenerator;

    public YardController(IService<YardResponse, YardDTO> service, LinkGenerator linkGenerator)
    {
        _service = service;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    /// Adiciona um novo Pátio
    /// </summary>
    /// <returns>O Pátio persistido</returns>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     POST /api/yard
    ///     {
    ///         "name": "Osasco I",
    ///         "subsidiaryId": 1 
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Retorna o Pátio criado</response>
    /// <response code="400">Retorna a mensagem de erro</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<YardResponse>> Create(YardDTO yardDTO)
    {
        var yard = await _service.Save(yardDTO);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Yard", new { id = yard.Id }),
            "self",
            "GET"
        );
        yard.Links.Add(link);
        return CreatedAtAction(nameof(ReadById), new { id = yard.Id }, yard);
    }

    /// <summary>
    /// Busca todos os Pátios
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>Uma página de Pátios</returns>
    /// <response code="200">Retorna uma página de Pátios</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<YardResponse>>> ReadAll(int page, int pageSize)
    {
        var yards = (await _service.FindAll(page, pageSize)).ToList();

        foreach (var yard in yards)
        {
            var link = new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Yard", new { id = yard.Id }),
                "self",
                "GET"
            );
            yard.Links.Add(link);
        }
        
        return Ok(yards);
    }

    /// <summary>
    /// Busca um Pátio pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>O Pátio encontrado</returns>
    /// <response code="200">Retorna o Pátio encontrado</response>
    /// <response code="404">Retorna caso o Pátio não seja encontrado</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<YardResponse>> ReadById(int id)
    {
        var yard = await _service.FindById(id);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Yard", new {}),
            "Lista de pátios",
            "GET"
        );
        yard.Links.Add(link);
        return Ok(yard);
    }

    /// <summary>
    /// Atualiza um Pátio pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     PUT /api/yard/1
    ///     {    
    ///         "name": "Barueri I",
    ///         "subsidiaryId": 1 
    ///     }
    /// 
    /// </remarks>
    /// <returns>O Pátio atualizado</returns>
    /// <response code="200">Retorna o Pátio atualizado</response>
    /// <response code="404">Retorna caso o Pátio não seja encontrado</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<YardResponse>> Update(int id, YardDTO yardDTO)
    {
        var yard = await _service.Update(id, yardDTO);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Yard", new { id = yard.Id }),
            "self",
            "GET"
        );
        yard.Links.Add(link);
        return Ok(yard);
    }

    /// <summary>
    /// Deleta um Pátio pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">Retorna caso o Pátio seja deletado com sucesso</response>
    /// <response code="404">Retorna caso o Pátio não seja encontrado</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<YardResponse>> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}