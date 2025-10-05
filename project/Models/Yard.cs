using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models;

public class Yard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int SubsidiaryId { get; set; }
    
    public string Name { get; set; }
    public Subsidiary Subsidiary { get; set; }
    public List<Bike> Bikes { get; set; }
}