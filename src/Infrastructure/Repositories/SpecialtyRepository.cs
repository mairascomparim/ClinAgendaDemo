using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly MySqlConnection _connection;

        public SpecialtyRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<SpecialtyDTO?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    ID, 
                    NAME,
                    SCHEDULEDURATION 
                FROM SPECIALTY
                WHERE ID = @Id";

            var specialty = await _connection.QueryFirstOrDefaultAsync<SpecialtyDTO>(query, new { Id = id });

            return specialty;
        }
        public async Task<(int total, IEnumerable<SpecialtyDTO> specialtys)> GetAllAsync(string? name, int? itemsPerPage, int? page)
        {
            var queryBase = new StringBuilder(@"
                FROM SPECIALTY S WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(name))
            {
                queryBase.Append(" AND S.NAME LIKE @Name");
                parameters.Add("Name", $"%{name}%");
            }

            var countQuery = $"SELECT COUNT(DISTINCT S.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
            SELECT ID, 
            NAME, 
            SCHEDULEDURATION
            {queryBase}
            LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            var specialtys = await _connection.QueryAsync<SpecialtyDTO>(dataQuery, parameters);

            return (total, specialtys);
        }
        public async Task<int> InsertSpecialtyAsync(SpecialtyInsertDTO specialtyInsertDTO)
        {
            string query = @"
            INSERT INTO SPECIALTY (NAME, SCHEDULEDURATION) 
            VALUES (@Name, @Scheduleduration);
            SELECT LAST_INSERT_ID();";
            return await _connection.ExecuteScalarAsync<int>(query, specialtyInsertDTO);
        }
        public async Task<int> DeleteSpecialtyAsync(int id)
        {
            string query = @"
            DELETE FROM SPECIALTY
            WHERE ID = @Id";

            var parameters = new { Id = id };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }
        public async Task<IEnumerable<SpecialtyDTO>> GetSpecialtiesByIds(List<int> specialtiesId)
        {
            var query = @"
                SELECT 
                    S.ID, 
                    S.NAME,
                    S.SCHEDULEDURATION 
                FROM SPECIALTY S
                WHERE S.ID IN @SPECIALTIESID";

            var parameters = new { SpecialtiesID = specialtiesId };

            return await _connection.QueryAsync<SpecialtyDTO>(query, parameters);
        }
    }

}