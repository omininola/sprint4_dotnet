using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sprint4.Models;

public class Bike
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int YardId { get; set; }
    
    public string Plate { get; set; }
    public string Model { get; set; }    
    public string Status { get; set; }
    public Yard Yard { get; set; }
}