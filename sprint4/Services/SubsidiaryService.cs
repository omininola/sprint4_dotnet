using sprint4.Data;
using sprint4.DTO.Subsidiary;
using sprint4.Exceptions;
using sprint4.Models;

namespace sprint4.Services;

public class SubsidiaryService : IService<SubsidiaryResponse, SubsidiaryDTO>
{
    private readonly AppDbContext _context;

    private readonly string NOT_FOUND_MESSAGE = "Filial com esse ID não foi encontrada.";
    
    public SubsidiaryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SubsidiaryResponse> Save(SubsidiaryDTO subsidiaryDTO)
    {
        var subsidiary = new Subsidiary
        {
            Name = subsidiaryDTO.Name,
            Address = subsidiaryDTO.Address
        };
        
        _context.Subsidiaries.Add(subsidiary);
        await _context.SaveChangesAsync();

        return new SubsidiaryResponse
        {
            Id = subsidiary.Id,
            Name = subsidiaryDTO.Name,
            Address = subsidiaryDTO.Address
        };
    }
    
    public async Task<IEnumerable<SubsidiaryResponse>> FindAll(int page = 1, int pageSize = 10)
    {
        var subsidiaries = _context.Subsidiaries.Skip((page - 1) * pageSize).Take(pageSize);
        var response = new List<SubsidiaryResponse>();
        
        foreach (var subsidiary in subsidiaries)
        {
            var subsidiaryResponse = new SubsidiaryResponse
            {
                Id = subsidiary.Id,
                Name = subsidiary.Name,
                Address = subsidiary.Address
            };
            response.Add(subsidiaryResponse);
        }

        return response;
    }

    public async Task<SubsidiaryResponse> FindById(int id)
    {
        var subsidiary = await _context.Subsidiaries.FindAsync(id);
        if (subsidiary == null) throw new NotFoundException(NOT_FOUND_MESSAGE);

        return new SubsidiaryResponse
        {
            Id = subsidiary.Id,
            Name = subsidiary.Name,
            Address = subsidiary.Address
        };
    }

    public async Task<SubsidiaryResponse> Update(int id, SubsidiaryDTO subsidiaryDTO)
    {
        var subsidiary = await _context.Subsidiaries.FindAsync(id);
        if (subsidiary == null) throw new NotFoundException(NOT_FOUND_MESSAGE);

        subsidiary.Name = subsidiaryDTO.Name;
        subsidiary.Address = subsidiaryDTO.Address;
        await _context.SaveChangesAsync();

        return new SubsidiaryResponse
        {
            Id = subsidiary.Id,
            Name = subsidiary.Name,
            Address = subsidiary.Address
        };
    }

    public async Task<SubsidiaryResponse> Delete(int id)
    {
        var subsidiary = await _context.Subsidiaries.FindAsync(id);
        if (subsidiary == null) throw new NotFoundException(NOT_FOUND_MESSAGE);
        
        _context.Subsidiaries.Remove(subsidiary);
        await _context.SaveChangesAsync();

        return new SubsidiaryResponse();
    }
}