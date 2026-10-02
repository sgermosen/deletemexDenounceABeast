namespace DenounceBeasts.API.Models.Dtos
{
    public class CreateSectorDTO
    {
        public string Name { get; set; } = string.Empty;
        public int MunicipalityId { get; set; }
    }
}
