using Microsoft.EntityFrameworkCore;
using project.Data;
using project.DTO.Bike;
using project.Exceptions;
using project.Models;

namespace project.Services;

public class BikeService : IService<BikeResponse, BikeDTO>
{
    private readonly AppDbContext _context;

    private readonly string NOT_FOUND_MESSAGE = "Moto com esse ID não foi encontrada.";
    private readonly string YARD_NOT_FOUND_MESSAGE = "Pátio com esse ID não foi encontrado.";
    
    public BikeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BikeResponse> Save(BikeDTO bikeDTO)
    {
        var yard = await _context.Yards.FindAsync(bikeDTO.YardId);
        if  (yard == null) throw new NotFoundException(YARD_NOT_FOUND_MESSAGE);

        var bike = new Bike
        {
            Plate = bikeDTO.Plate,
            Model = bikeDTO.Model,
            Status = bikeDTO.Status,
            YardId = bikeDTO.YardId,
            Yard = yard
        };
        
        _context.Bikes.Add(bike);
        await _context.SaveChangesAsync();

        return new BikeResponse
        {
            Id = bike.Id,
            Plate = bike.Plate,
            Model = bike.Model,
            Status = bike.Status,
            YardName = yard.Name
        };
    }
    
    public async Task<IEnumerable<BikeResponse>> FindAll(int page = 1, int pageSize = 10)
    {
        var bikes = _context.Bikes
            .Select(b => new BikeResponse
            {
                Id = b.Id,
                Plate = b.Plate,
                Model = b.Model,
                Status = b.Status,
                YardName = b.Yard.Name
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return bikes;
    }

    public async Task<BikeResponse> FindById(int id)
    {
        var bike = await _context.Bikes
            .Select(b => new BikeResponse
            {
                Id = b.Id,
                Plate = b.Plate,
                Model = b.Model,
                Status = b.Status,
                YardName = b.Yard.Name
            })
            .Where(b => b.Id == id)
            .FirstAsync();
        if (bike == null) throw new NotFoundException(NOT_FOUND_MESSAGE);

        return bike;
    }

    public async Task<BikeResponse> Update(int id, BikeDTO bikeDTO)
    {
        var yard = await _context.Yards.FindAsync(bikeDTO.YardId);
        if (yard == null) throw new NotFoundException(YARD_NOT_FOUND_MESSAGE);
        
        var bike = await _context.Bikes.FindAsync(id);
        if (bike == null) throw new NotFoundException(NOT_FOUND_MESSAGE);
        
        bike.Plate = bikeDTO.Plate;
        bike.Model = bikeDTO.Model;
        bike.Status = bikeDTO.Status;
        bike.YardId = bikeDTO.YardId;
        bike.Yard = yard;
        await _context.SaveChangesAsync();

        return new BikeResponse
        {
            Id = bike.Id,
            Plate = bike.Plate,
            Model = bike.Model,
            Status = bike.Status,
            YardName = yard.Name
        };
    }

    public async Task<BikeResponse> Delete(int id)
    {
        var bike = await _context.Bikes.FindAsync(id);
        if (bike == null) throw new NotFoundException(NOT_FOUND_MESSAGE);
        
        _context.Bikes.Remove(bike);
        await _context.SaveChangesAsync();

        return new BikeResponse();
    }
}