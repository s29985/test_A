namespace test_A.DTO.Patients.Get;

public class PatientDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Phone { get; set; } = null!;
    public List<AppointmentDto> Appointments { get; set; } = new();
}
