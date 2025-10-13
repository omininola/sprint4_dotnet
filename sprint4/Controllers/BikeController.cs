using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.DTO.Bike;
using project.HATEOAS;
using project.Services;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikeController : Controller
{
    private readonly IService<BikeResponse, BikeDTO> _service;
    private readonly LinkGenerator _linkGenerator;

    public BikeController(IService<BikeResponse, BikeDTO> service, LinkGenerator linkGenerator)
    {
        _service = service;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    /// Adiciona uma nova Moto
    /// </summary>
    /// <returns>A Moto persistida</returns>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     POST /api/bike
    ///     {
    ///         "plate": "123ABC",
    ///         "model": "SPORT",
    ///         "status": "READY",
    ///         "yardId": 1
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Retorna a Moto criada</response>
    /// <response code="400">Retorna a mensagem de erro</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BikeResponse>> Create(BikeDTO bikeDTO)
    {
        var bike = await _service.Save(bikeDTO);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Bike", new { id = bike.Id }),
            "self",
            "GET"
        );
        bike.Links.Add(link);
        return CreatedAtAction(nameof(ReadById), new { id = bike.Id }, bike);
    }

    /// <summary>
    /// Busca todas as Motos
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>Uma página de Motos</returns>
    /// <response code="200">Retorna uma página de Motos</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<BikeResponse>>> ReadAll(int page, int pageSize)
    {
        var bikes = (await _service.FindAll(page, pageSize)).ToList();

        foreach (var bike in bikes)
        {
            var link = new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Bike", new { id = bike.Id }),
                "self",
                "GET"
            );
            bike.Links.Add(link);
        }
        
        return Ok(bikes);
    }

    /// <summary>
    /// Busca uma Moto pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A Moto encontrada</returns>
    /// <response code="200">Retorna a Moto encontrada</response>
    /// <response code="404">Retorna caso a Moto não seja encontrada</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<BikeResponse>> ReadById(int id)
    {
        var bike = await _service.FindById(id);

        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadAll), "Bike"),
            "Lista de motos",
            "GET"
        );
        bike.Links.Add(link);
        return Ok(bike);
    }

    /// <summary>
    /// Atualiza uma Moto pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     PUT /api/bike/1
    ///     {
    ///         "plate": "123ABC",
    ///         "model": "SPORT",
    ///         "status": "BROKEN",
    ///         "yardId": 1
    ///     }
    /// 
    /// </remarks>
    /// <returns>A Moto atualizada</returns>
    /// <response code="200">Retorna a Moto atualizada</response>
    /// <response code="404">Retorna caso a Moto não seja encontrada</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<BikeResponse>> Update(int id, BikeDTO bikeDTO)
    {
        var bike = await _service.Update(id, bikeDTO);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Bike", new { id = bike.Id }),
            "self",
            "GET"
        );
        bike.Links.Add(link);
        return Ok(bike);
    }

    /// <summary>
    /// Deleta uma Moto pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">Retorna caso a Moto seja deletada com sucesso</response>
    /// <response code="404">Retorna caso a Moto não seja encontrada</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<BikeResponse>> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}