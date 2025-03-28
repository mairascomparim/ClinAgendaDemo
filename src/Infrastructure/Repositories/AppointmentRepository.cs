using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Appointment;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly MySqlConnection _connection;

        public AppointmentRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<(int total, IEnumerable<AppointmentListDTO> appointment)> GetAppointmentsAsync(string? patientName, string? doctorName, int? specialtyId, int itemsPerPage, int page)
        {
            var queryBase = new StringBuilder(@"     
                   FROM Appointment A
                    INNER JOIN PATIENT P ON P.ID = A.PATIENTID
                    INNER JOIN DOCTOR D ON D.ID = A.DOCTORID
                    INNER JOIN SPECIALTY S ON S.ID = A.SPECIALTYID ");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(patientName))
            {
                queryBase.Append(" AND P.NAME LIKE @Name");
                parameters.Add("Name", $"%{patientName}%");
            }

            if (!string.IsNullOrEmpty(doctorName))
            {
                queryBase.Append(" AND D.NAME LIKE @DoctorName");
                parameters.Add("DoctorName", $"%{doctorName}%");
            }

            if (specialtyId.HasValue)
            {
                queryBase.Append(" AND S.ID = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT A.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
            SELECT 
            A.ID,
            P.NAME AS PATIENTNAME,
            P.DOCUMENTNUMBER AS PATIENTDOCUMENT,
            D.NAME AS DOCTORNAME,
            S.ID AS SPECIALTYID,
            S.NAME AS SPECIALTYNAME,
            S.SCHEDULEDURATION AS SCHEDULEDURATION,
            A.APPOINTMENTDATE AS APPOINTMENTDATE
        {queryBase}
        ORDER BY A.ID
        LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            var appointment = await _connection.QueryAsync<AppointmentListDTO>(dataQuery, parameters);

            return (total, appointment);
        }
        public async Task<int> InsertAppointmentAsync(AppointmentDTO appointment)
        {
            string query = @"
            INSERT INTO Appointment (patientId, doctorId, specialtyId, appointmentDate, observation)
            VALUES (@patientId, @doctorId, @specialtyId, @appointmentDate, @observation);
            SELECT LAST_INSERT_ID();";
            return await _connection.ExecuteScalarAsync<int>(query, appointment);
        }
        public async Task<AppointmentDTO?> GetByIdAsync(int id)
        {
            string query = "SELECT * FROM Appointment WHERE Id = @Id;";
            return await _connection.QueryFirstOrDefaultAsync<AppointmentDTO>(query, new { Id = id });
        }
        public async Task<bool> UpdateAsync(AppointmentInsertDTO patient)
        {
            string query = @"
            UPDATE Appointment SET 
                patientId = @PatientId,
                doctorId = @DoctorId,
                specialtyId = @SpecialtyId,
                appointmentDate = @AppointmentDate,
                observation = @Observation
            WHERE Id = @Id;";
            int rowsAffected = await _connection.ExecuteAsync(query, patient);
            return rowsAffected > 0;
        }
        public async Task<int> DeleteAsync(int appointmentId)
        {
            string query = "DELETE FROM Appointment WHERE ID = @AppointmentId";
            var rowsAffected = await _connection.ExecuteAsync(query, new { AppointmentId = appointmentId });
            return rowsAffected;
        }
    }
}