using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DenounceBeasts.API.Models.Entities
{
    //[Table("MUNICIPALITY")]
    public class MunicipalityDto
    {
        public int Id { get; set; }

        //[Column("MUNICIPALITY_NAME")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? PostalCode { get; set; }
        public bool IsActive { get; set; } = true;
       public List<Sector> Sectors { get; set; }
    }

}
