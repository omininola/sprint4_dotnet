using sprint4.HATEOAS;
using sprint4.DTO.Bike;

namespace sprint4.DTO.Yard;

public class YardResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SubsidiaryName { get; set; }
    public List<BikeResponse> Bikes { get; set; }
    public List<Link> Links { get; set; } = new List<Link>();
}