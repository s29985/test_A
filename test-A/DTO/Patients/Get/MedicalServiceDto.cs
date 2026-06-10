namespace test_A.DTO.Patients.Get;

public class MedicalServiceDto
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
}
