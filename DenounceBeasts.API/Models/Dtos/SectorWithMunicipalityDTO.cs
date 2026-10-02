namespace DenounceBeasts.API.Models.Dtos
{
    public class SectorWithMunicipalityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }

    }
}
