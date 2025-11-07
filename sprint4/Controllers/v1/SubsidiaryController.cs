using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sprint4.DTO.Subsidiary;
using sprint4.HATEOAS;
using sprint4.Services;

namespace sprint4.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SubsidiaryController : Controller
{
    private readonly IService<SubsidiaryResponse, SubsidiaryDTO> _service;
    private readonly PredictionService _predictionService;
    
    private readonly LinkGenerator _linkGenerator;

    public SubsidiaryController(IService<SubsidiaryResponse, SubsidiaryDTO> service,  PredictionService predictionService, LinkGenerator linkGenerator)
    {
        _service = service;
        _predictionService = predictionService;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    /// Adiciona uma nova Filial
    /// </summary>
    /// <returns>A Filial persistida</returns>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     POST /api/subsidiary
    ///     {
    ///         "name": "Osasco",
    ///         "address": "Rua dos Bobos, 123"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Retorna a Filial criada</response>
    /// <response code="400">Retorna a mensagem de erro</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<SubsidiaryResponse>> Create(SubsidiaryDTO subsidiaryDTO)
    {
        var subsidiary = await _service.Save(subsidiaryDTO);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Subsidiary", new { id = subsidiary.Id }),
            "self",
            "GET"
        );
        subsidiary.Links.Add(link);
        return CreatedAtAction(nameof(ReadById), new { id = subsidiary.Id }, subsidiary);
    }

    /// <summary>
    /// Busca todas as Filiais
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>Uma página de Filiais</returns>
    /// <response code="200">Retorna uma página de Filiais</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SubsidiaryResponse>>> ReadAll(int page, int pageSize)
    {
        var subsidiaries = (await _service.FindAll(page, pageSize)).ToList();
        
        foreach (var subsidiary in subsidiaries)
        {
            var link = new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Subsidiary", new { id = subsidiary.Id }),
                "self",
                "GET"
            );
            subsidiary.Links.Add(link);
        }
        
        return Ok(subsidiaries);
    }

    /// <summary>
    /// Busca uma Filial pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A Filial encontrada</returns>
    /// <response code="200">Retorna a Filial encontrada</response>
    /// <response code="404">Retorna caso a Filial não seja encontrada</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<SubsidiaryResponse>> ReadById(int id)
    {
        var subsidiary = await _service.FindById(id);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadAll), "Subsidiary", new {}),
            "Lista de filiais",
            "GET"
        );
        subsidiary.Links.Add(link);
        return Ok(subsidiary);
    }

    /// <summary>
    /// Atualiza uma Filial pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     PUT /api/subsidiary/1
    ///     {
    ///         "name": "Barueri",
    ///         "address": "Rua dos Bobos, 456"
    ///     }
    /// 
    /// </remarks>
    /// <returns>A Filial atualizada</returns>
    /// <response code="200">Retorna a Filial atualizada</response>
    /// <response code="404">Retorna caso a Filial não seja encontrada</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<SubsidiaryResponse>> Update(int id, SubsidiaryDTO subsidiaryDTO)
    {
        var subsidiary = await _service.Update(id, subsidiaryDTO);
        var link = new Link(
            _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Subsidiary", new { id = subsidiary.Id }),
            "self",
            "GET"
        );
        subsidiary.Links.Add(link);
        return Ok(subsidiary);
    }

    /// <summary>
    /// Deleta uma Filial pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">Retorna caso a Filial seja deletada com sucesso</response>
    /// <response code="404">Retorna caso a Filial não seja encontrada</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<SubsidiaryResponse>> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// Retorna uma aproximação de quantas motos cabem em uma filial com base em seu metro quadrado
    /// </summary>
    /// <returns>O número motos aproximado</returns>
    /// <response code="200">Retorna as informações com base no metro quadrado</response>
    [HttpGet("predict")]
    [Authorize]
    public async Task<ActionResult<SubsidiaryPredictionResponse>> Predict([FromQuery] float? metros)
    {
        if (metros == null || metros <= 0)
            BadRequest("Os metros quadrados da filial devem ser passados para realizarmos a aproximação");
        
        var result = _predictionService.Predict(metros.GetValueOrDefault());
        var rounded = (int)Math.Round(result.PredictedMotorcycles);
        
        var response = new SubsidiaryPredictionResponse
        {
            Area = metros.GetValueOrDefault(),
            Prediction = result.PredictedMotorcycles,
            Rounded = rounded
        };
        
        return Ok(response);
    }
}