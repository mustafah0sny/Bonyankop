using Microsoft.AspNetCore.Mvc;

namespace BonyankopAPI.Controllers
{
    /// <summary>
    /// Health check endpoints for monitoring API and database status
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Check if the API is running
        /// </summary>
        /// <returns>API status and version information</returns>
        /// <response code="200">API is healthy and running</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                message = "Bonyankop API is running"
            });
        }

        /// <summary>
        /// Check database connection status
        /// </summary>
        /// <returns>Database connection status</returns>
        /// <response code="200">Database is connected</response>
        /// <response code="503">Database is disconnected or error occurred</response>
        [HttpGet("db")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<object>> GetDatabaseHealth([FromServices] Data.ApplicationDbContext context)
        {
            try
            {
                // Try to connect to database with timeout
                var canConnect = await context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    return Ok(new
                    {
                        status = "healthy",
                        database = "connected",
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return StatusCode(503, new
                    {
                        status = "unhealthy",
                        database = "disconnected",
                        message = "Cannot connect to database",
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    database = "error",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
