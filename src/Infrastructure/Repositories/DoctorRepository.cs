using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly MySqlConnection _connection;

        public DoctorRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<(int total, IEnumerable<DoctorListDTO> doctors)> GetDoctorsAsync(string? name, int? specialtyId, int? statusId, int offset, int itemsPerPage)
        {

            var innerJoins = new StringBuilder(@"
                FROM DOCTOR D
                INNER JOIN STATUS S ON D.STATUSID = S.ID
                INNER JOIN DOCTOR_SPECIALTY DSPE ON DSPE.DOCTORID = D.ID
                WHERE 1 = 1 ");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(name))
            {
                innerJoins.Append(" AND D.NAME LIKE @Name");
                parameters.Add("Name", $"%{name}%");
            }

            if (specialtyId.HasValue)
            {
                innerJoins.Append(" AND DSPE.SPECIALTYID = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (statusId.HasValue)
            {
                innerJoins.Append(" AND S.ID = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT D.ID) {innerJoins}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);
            
            parameters.Add("LIMIT", itemsPerPage);
            parameters.Add("OFFSET", offset);

            var query = $@"
                SELECT DISTINCT
                    D.ID AS ID,
                    D.NAME AS NAME,
                    S.ID AS STATUSID,
                    S.NAME AS STATUSNAME
                {innerJoins}
                ORDER BY D.ID
                LIMIT @Limit OFFSET @Offset";

                var doctors = await _connection.QueryAsync<DoctorListDTO>(query.ToString(), parameters);

                return (total, doctors);
        }

        public async Task<IEnumerable<SpecialtyDoctorDTO>> GetDoctorSpecialtiesAsync(int[] doctorIds)
        {
            var query = @"
                SELECT 
                    DS.DOCTORID AS DOCTORID,
                    SP.ID AS SPECIALTYID,
                    SP.NAME AS SPECIALTYNAME,
                    SP.SCHEDULEDURATION 
                FROM DOCTOR_SPECIALTY DS
                INNER JOIN SPECIALTY SP ON DS.SPECIALTYID = SP.ID
                WHERE DS.DOCTORID IN @DOCTORIDS";

            var parameters = new { DoctorIds = doctorIds };

            return await _connection.QueryAsync<SpecialtyDoctorDTO>(query, parameters);
        }
        public async Task<int> InsertDoctorAsync(DoctorInsertDTO doctor)
        {
            string query = @"
            INSERT INTO Doctor (Name, StatusId) 
            VALUES (@Name, @StatusId);
            SELECT LAST_INSERT_ID();";
            return await _connection.ExecuteScalarAsync<int>(query, doctor);
        }
        public async Task<IEnumerable<DoctorListDTO>> GetByIdAsync(int id)
        {
            var queryBase = new StringBuilder(@"
                    FROM DOCTOR D
                LEFT JOIN DOCTOR_SPECIALTY DSPE ON D.ID = DSPE.DOCTORID
                LEFT JOIN STATUS S ON S.ID = D.STATUSID
                LEFT JOIN SPECIALTY SP ON SP.ID = DSPE.SPECIALTYID
                    WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (id > 0)
            {
                queryBase.Append(" AND D.ID = @id");
                parameters.Add("id", id);
            }

            var dataQuery = $@"
        SELECT DISTINCT
            D.ID, 
            D.NAME, 
            D.STATUSID AS STATUSID, 
            S.NAME AS STATUSNAME,
            DSPE.SPECIALTYID AS SPECIALTYID,
            SP.NAME AS SPECIALTYNAME
        {queryBase}
        ORDER BY D.ID";

            var doctors = await _connection.QueryAsync<DoctorListDTO>(dataQuery, parameters);

            return doctors;
        }
        public async Task<bool> UpdateAsync(DoctorDTO doctor)
        {
            string query = @"
            UPDATE Doctor SET 
                Name = @Name,
                StatusId = @StatusId
            WHERE Id = @Id;";
            int rowsAffected = await _connection.ExecuteAsync(query, doctor);
            return rowsAffected > 0;
        }
        public async Task<int> DeleteByDoctorIdAsync(int id)
        {
            string query = "DELETE FROM DOCTOR WHERE ID = @Id";

            var parameters = new { Id = id };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }

    }
}