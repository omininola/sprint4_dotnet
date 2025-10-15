using System.ComponentModel.DataAnnotations;

namespace sprint4.DTO.Bike;

public class BikeDTO
{
    [Required(ErrorMessage = "A placa da moto é obrigatória")]
    public string Plate { get; set; }
    
    [Required(ErrorMessage = "O modelo da moto é obrigatório")]
    public string Model { get; set; }
    
    [Required(ErrorMessage = "O status da moto é obrigatório")]
    public string Status { get; set; }

    [Required(ErrorMessage = "O ID do pátio é obrigatório")]
    public int YardId { get; set; }
}