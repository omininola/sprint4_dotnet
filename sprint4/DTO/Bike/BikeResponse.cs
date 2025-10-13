using project.HATEOAS;

namespace project.DTO.Bike;

public class BikeResponse
{
    public int Id { get; set; }
    public string Plate { get; set; }
    public string Model { get; set; }    
    public string Status { get; set; }
    public string YardName { get; set; }
    public List<Link> Links { get; set; } = new List<Link>();
}