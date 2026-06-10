namespace test_A.DTO.Patients.Get;

public class AppointmentServiceDto
{
    public int Quantity { get; set; }
    public DateOnly PerformedAt { get; set; }
    public MedicalServiceDto MedicalService { get; set; } = null!;
}
