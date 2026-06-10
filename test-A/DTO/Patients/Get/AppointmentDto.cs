namespace test_A.DTO.Patients.Get;

public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public DoctorDto Doctor { get; set; } = null!;
    public DateOnly AppointmentDate { get; set; }
    public string Status { get; set; } = null!;
    public List<AppointmentServiceDto> AppointmentServices { get; set; } = new();
}
