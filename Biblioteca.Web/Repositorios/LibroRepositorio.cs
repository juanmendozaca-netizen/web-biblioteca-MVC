using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Web.Repositorios
{
    public class LibroRepositorio
    {
        private readonly string _connectionString;

        public LibroRepositorio(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<Libro>> GetAllAsync(string? titulo)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var parameters = new { Titulo = titulo };
            return await conn.QueryAsync<Libro>("usp_Libros_ListarActivos", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<Libro?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var libro = await conn.QueryFirstOrDefaultAsync<Libro>("usp_Libros_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
            return libro;
        }

        public async Task<int> CreateAsync(Libro libro)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var parameters = new DynamicParameters();
            parameters.Add("Titulo", libro.Titulo);
            parameters.Add("AutorId", libro.AutorId);
            // El stored procedure no espera Genero/Editorial en la versión actual
            parameters.Add("Anio", libro.Anio);
            parameters.Add("Ejemplares", libro.Ejemplares);
            parameters.Add("NuevoId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("usp_Libros_Insertar", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("NuevoId");
        }

        public async Task<int> UpdateAsync(Libro libro)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var affected = await conn.ExecuteAsync("usp_Libros_Actualizar", new
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                AutorId = libro.AutorId,
                Anio = libro.Anio,
                Ejemplares = libro.Ejemplares
            }, commandType: CommandType.StoredProcedure);
            return affected;
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var affected = await conn.ExecuteAsync("usp_Libros_EliminarLogico", new { Id = id }, commandType: CommandType.StoredProcedure);
            return affected;
        }

        public async Task<IEnumerable<Models.Autor>> GetAutoresAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return await conn.QueryAsync<Models.Autor>("usp_Autores_ListarActivos", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetCountAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "SELECT COUNT(1) FROM Libros WHERE Activo = 1";
            return await conn.ExecuteScalarAsync<int>(sql);
        }
    }
}
