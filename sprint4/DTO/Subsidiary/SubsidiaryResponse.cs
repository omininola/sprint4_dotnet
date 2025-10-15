using sprint4.HATEOAS;
using sprint4.DTO.Yard;

namespace sprint4.DTO.Subsidiary;

public class SubsidiaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<YardResponse> Yards { get; set; }
    public List<Link> Links { get; set; } = new List<Link>();
}