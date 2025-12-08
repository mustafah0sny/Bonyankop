namespace BonyankopAPI.DTOs;

public class CostBreakdownDto
{
    public decimal LaborCost { get; set; }
    public decimal MaterialsCost { get; set; }
    public decimal EquipmentCost { get; set; }
    public decimal OtherCosts { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
