using Microsoft.EntityFrameworkCore;
using project.Data;
using project.DTO.Yard;
using project.Exceptions;
using project.Models;

namespace project.Services;

public class YardService : IService<YardResponse, YardDTO>
{
    private readonly AppDbContext _context;

    private readonly string NOT_FOUND_MESSAGE = "Pátio com esse ID não foi encontrado.";
    private readonly string SUBSIDIARY_NOT_FOUND_MESSAGE = "Filial com esse ID não foi encontrada.";
    
    public YardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<YardResponse> Save(YardDTO yardDTO)
    {
        var subsidiary = await _context.Subsidiaries.FindAsync(yardDTO.SubsidiaryId);
        if  (subsidiary == null) throw new NotFoundException(SUBSIDIARY_NOT_FOUND_MESSAGE);

        var yard = new Yard
        {
            Name = yardDTO.Name,
            SubsidiaryId = yardDTO.SubsidiaryId,
            Subsidiary = subsidiary
        };
        
        _context.Yards.Add(yard);
        await _context.SaveChangesAsync();

        return new YardResponse
        {
            Id = yard.Id,
            Name = yard.Name,
            SubsidiaryName = subsidiary.Name
        };
    }
    
    public async Task<IEnumerable<YardResponse>> FindAll(int page = 1, int pageSize = 10)
    {
        var yards = _context.Yards
            .Select(y => new YardResponse
            {
                Id = y.Id,
                Name = y.Name,
                SubsidiaryName = y.Subsidiary.Name   
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return yards;
    }

    public async Task<YardResponse> FindById(int id)
    {
        var yard = await _context.Yards
            .Select(y => new YardResponse
            {
                Id = y.Id,
                Name = y.Name,
                SubsidiaryName = y.Subsidiary.Name
            })
            .Where(y => y.Id == id)
            .FirstAsync();
        if (yard == null) throw new NotFoundException(NOT_FOUND_MESSAGE);
        
        return yard;
    }

    public async Task<YardResponse> Update(int id, YardDTO yardDTO)
    {
        var subsidiary = await _context.Subsidiaries.FindAsync(yardDTO.SubsidiaryId);
        if (subsidiary == null) throw new NotFoundException(SUBSIDIARY_NOT_FOUND_MESSAGE);
        
        var yard = await _context.Yards.FindAsync(id);
        if (yard == null) throw new NotFoundException(NOT_FOUND_MESSAGE);
        
        yard.Name = yardDTO.Name;
        yard.SubsidiaryId = yardDTO.SubsidiaryId;
        yard.Subsidiary = subsidiary;
        await _context.SaveChangesAsync();

        return new YardResponse
        {
            Id = yard.Id,
            Name = yard.Name,
            SubsidiaryName = subsidiary.Name
        };
    }

    public async Task<YardResponse> Delete(int id)
    {
        var yard = await _context.Yards.FindAsync(id);
        if (yard == null) throw new NotFoundException(NOT_FOUND_MESSAGE);
        
        _context.Yards.Remove(yard);
        await _context.SaveChangesAsync();

        return new YardResponse();
    }
}