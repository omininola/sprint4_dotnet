using project.HATEOAS;
using project.DTO.Bike;

namespace project.DTO.Yard;

public class YardResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SubsidiaryName { get; set; }
    public List<BikeResponse> Bikes { get; set; }
    public List<Link> Links { get; set; } = new List<Link>();
}