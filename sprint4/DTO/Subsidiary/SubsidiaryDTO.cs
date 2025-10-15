using System.ComponentModel.DataAnnotations;

namespace sprint4.DTO.Subsidiary;

public class SubsidiaryDTO
{
    [Required(ErrorMessage = "O nome da filial é obrigatório")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "O endereço da filial é obrigatório")]
    public string Address { get; set; }
}