using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Web.Repositorios
{
    public class PrestamoRepositorio
    {
        private readonly string _connectionString;

        public PrestamoRepositorio(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<PrestamoReporte>> GetReporteAsync(DateTime desde, DateTime hasta)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return await conn.QueryAsync<PrestamoReporte>("usp_Prestamos_ReportePorFechas", new { Desde = desde, Hasta = hasta }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetPrestamosActivosCountAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "SELECT COUNT(1) FROM Prestamos WHERE Estado = 'Prestado'";
            return await conn.ExecuteScalarAsync<int>(sql);
        }

        public async Task<int> GetPrestamosVencidosCountAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "SELECT COUNT(1) FROM Prestamos WHERE Estado = 'Prestado' AND FechaLimite < GETDATE()";
            return await conn.ExecuteScalarAsync<int>(sql);
        }

        public async Task<IEnumerable<Models.PrestamoMonthly>> GetPrestamosMonthlyAsync(int monthsBack = 6)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = @"
SELECT YEAR(FechaPrestamo) AS [Year], MONTH(FechaPrestamo) AS [Month], COUNT(1) AS [Count]
FROM Prestamos
WHERE FechaPrestamo >= DATEADD(MONTH, @MonthsBack - 1, GETDATE())
GROUP BY YEAR(FechaPrestamo), MONTH(FechaPrestamo)
ORDER BY YEAR(FechaPrestamo), MONTH(FechaPrestamo);
";
            var list = await conn.QueryAsync<Models.PrestamoMonthly>(sql, new { MonthsBack = monthsBack });
            // Ensure months with zero are represented
            var result = new List<Models.PrestamoMonthly>();
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(monthsBack - 1));
            for (int i = 0; i < monthsBack; i++)
            {
                var dt = start.AddMonths(i);
                var item = list.FirstOrDefault(x => x.Year == dt.Year && x.Month == dt.Month);
                if (item == null)
                {
                    result.Add(new Models.PrestamoMonthly { Year = dt.Year, Month = dt.Month, Count = 0 });
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
