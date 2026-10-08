using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Controllers
{
    public class DbTestController : Controller
    {
        private readonly IConfiguration _config;

        public DbTestController(IConfiguration config)
        {
            _config = config;
        }

        // GET: /DbTest/Test
        public async Task<IActionResult> Test()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                return StatusCode(500, "Connection string 'DefaultConnection' no encontrada en configuration.");
            }

            try
            {
                await using var conn = new SqlConnection(connStr);
                await conn.OpenAsync();
                // opcional: ejecutar una consulta sencilla
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT DB_NAME();";
                var dbName = (await cmd.ExecuteScalarAsync())?.ToString() ?? "(null)";
                await conn.CloseAsync();
                return Content($"OK - conexión abierta. Base de datos: {dbName}");
            }
            catch (Exception ex)
            {
                // Devuelve detalle de la excepción e inner exceptions para depuración
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Exception: " + ex.GetType().FullName);
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace ?? "(no stack)");

                var inner = ex.InnerException;
                while (inner != null)
                {
                    sb.AppendLine("--- Inner Exception ---");
                    sb.AppendLine("Type: " + inner.GetType().FullName);
                    sb.AppendLine("Message: " + inner.Message);
                    sb.AppendLine(inner.StackTrace ?? "(no stack)");
                    inner = inner.InnerException;
                }

                return StatusCode(500, sb.ToString());
            }
        }
    }
}
