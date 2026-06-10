namespace test_A.DTO.Patients.Post;

public class CreatePatientRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Phone { get; set; } = null!;
    public CreatePatientAppointmentRequest Appointment { get; set; } = null!;
}

public class CreatePatientAppointmentRequest
{
    public int DoctorId { get; set; }
    public DateOnly AppointmentDate { get; set; }
}
