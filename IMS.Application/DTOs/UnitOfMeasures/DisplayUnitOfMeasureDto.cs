namespace IMS.Application.DTOs.UnitOfMeasures
{
    public class DisplayUnitOfMeasureDto
    {
        public Guid UnitOfMeasureId { get; set; }
        public string UnitName { get; set; }
        public string? Abbreviation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
