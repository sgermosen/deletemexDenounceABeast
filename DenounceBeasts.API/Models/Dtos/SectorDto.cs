namespace DenounceBeasts.API.Models.Dtos
{
    public class SectorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MunicipalityId { get; set; }
        public bool IsActive { get; set; } = true;
        
    }

}
