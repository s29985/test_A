using Microsoft.EntityFrameworkCore;
using test_A.DTO.Patients.Get;
using test_A.DTO.Patients.Post;
using test_A.Entities;
using test_A.Infrastructure;

namespace test_A.Services;

public class PatientService : IPatientService
{
    private readonly ClinicDbContext _db;

    public PatientService(ClinicDbContext db)
    {
        _db = db;
    }

    public async Task<List<PatientDto>> GetPatientsAsync(string? lastNameFilter, CancellationToken cancellationToken)
    {
        var query = _db.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Appointments)
                .ThenInclude(a => a.AppointmentServices)
                    .ThenInclude(x => x.MedicalService)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(lastNameFilter))
        {
            if (lastNameFilter.Length < 3)
            {
                return new List<PatientDto>();
            }

            query = query.Where(p => EF.Functions.Like(p.LastName, $"%{lastNameFilter}%"));
        }

        var result = await query
            .Select(p => new PatientDto
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                Phone = p.Phone,
                Appointments = p.Appointments
                    .OrderBy(a => a.AppointmentDate)
                    .Select(a => new AppointmentDto
                    {
                        AppointmentId = a.AppointmentId,
                        Doctor = new DoctorDto
                        {
                            FirstName = a.Doctor.FirstName,
                            LastName = a.Doctor.LastName,
                            Specialization = a.Doctor.Specialization,
                            Phone = a.Doctor.Phone
                        },
                        AppointmentDate = a.AppointmentDate,
                        Status = a.Status,
                        AppointmentServices = a.AppointmentServices
                            .OrderBy(x => x.PerformedAt)
                            .Select(x => new AppointmentServiceDto
                            {
                                Quantity = x.Quantity,
                                PerformedAt = x.PerformedAt,
                                MedicalService = new MedicalServiceDto
                                {
                                    ServiceId = x.MedicalService.ServiceId,
                                    Name = x.MedicalService.Name,
                                    Description = x.MedicalService.Description,
                                    Price = x.MedicalService.Price,
                                    DurationMinutes = x.MedicalService.DurationMinutes
                                }
                            }).ToList()
                    }).ToList()
            })
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<int> CreatePatientWithAppointmentAsync(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentException("Request body is required");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Length > 50)
        {
            throw new ArgumentException("FirstName is required and must be <= 50 characters");
        }
        if (string.IsNullOrWhiteSpace(request.LastName) || request.LastName.Length > 100)
        {
            throw new ArgumentException("LastName is required and must be <= 100 characters");
        }
        if (string.IsNullOrWhiteSpace(request.Phone) || request.Phone.Length != 9)
        {
            throw new ArgumentException("Phone must be exactly 9 characters");
        }

        if (request.Appointment is null)
        {
            throw new ArgumentException("Appointment data is required");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        if (request.Appointment.AppointmentDate < today)
        {
            throw new InvalidOperationException("Appointment date cannot be earlier than today");
        }

        var doctor = await _db.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DoctorId == request.Appointment.DoctorId, cancellationToken);

        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var patient = new Patient
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DateOfBirth = request.DateOfBirth,
                Phone = request.Phone.Trim()
            };

            await _db.Patients.AddAsync(patient, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            var appointment = new Appointment
            {
                PatientId = patient.PatientId,
                DoctorId = request.Appointment.DoctorId,
                AppointmentDate = request.Appointment.AppointmentDate,
                Status = "Scheduled"
            };

            await _db.Appointments.AddAsync(appointment, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return patient.PatientId;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
