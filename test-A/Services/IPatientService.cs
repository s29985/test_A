using test_A.DTO.Patients.Get;
using test_A.DTO.Patients.Post;

namespace test_A.Services;

public interface IPatientService
{
    Task<List<PatientDto>> GetPatientsAsync(string? lastNameFilter, CancellationToken cancellationToken);
    Task<int> CreatePatientWithAppointmentAsync(CreatePatientRequest request, CancellationToken cancellationToken);
}
