using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Web.Repositorios
{
    public class SocioRepositorio
    {
        private readonly string _connectionString;

        public SocioRepositorio(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<Socio>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return await conn.QueryAsync<Socio>("usp_Socios_ListarActivos", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Socio socio)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var parameters = new DynamicParameters();
            parameters.Add("Nombre", socio.Nombre);
            parameters.Add("DNI", socio.DNI);
            parameters.Add("Email", socio.Email);
            parameters.Add("NuevoId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("usp_Socios_Insertar", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("NuevoId");
        }

        public async Task<bool> DniExistsAsync(string dni)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var count = await conn.ExecuteScalarAsync<int>("usp_Socios_ExisteDNI", new { DNI = dni }, commandType: CommandType.StoredProcedure);
            return count > 0;
        }

        public async Task<int> GetCountAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "SELECT COUNT(1) FROM Socios WHERE Activo = 1";
            return await conn.ExecuteScalarAsync<int>(sql);
        }
    }
}
