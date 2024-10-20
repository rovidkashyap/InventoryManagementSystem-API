namespace IMS.Application.DTOs.UnitOfMeasures
{
    public class CreateUnitOfMeasureDto
    {
        public Guid UnitOfMeasureId { get; set; }
        public string UnitName { get; set; }
        public string? Abbreviation { get; set; }
    }
}
