using System.Data;
using System.Text;
using Npgsql;

namespace TodoApi.Services;

public class PostgresService(IConfiguration configuration, ILogger<PostgresService> logger)
{
    public readonly IConfiguration _configuration = configuration;
    public readonly ILogger _logger = logger;

    public async Task<string> FetchAsync(string query)
    {
        var connectionString = _configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("Connection string 'DbContext' not found.");
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        // Retrieve all rows
        query = @"select jsonb_agg((t.*)) from ( " + query + @" ) t";
        _logger.LogInformation("Executing query: {Query}", query);
        await using var cmd = dataSource.CreateCommand(query);
        await using var reader = await cmd.ExecuteReaderAsync();
        var stringBuilder = new StringBuilder();
        while (await reader.ReadAsync())
        {
            stringBuilder.AppendLine(reader.GetString(0));
            // _logger.LogInformation(reader.GetString(0));
        }
        return stringBuilder.ToString();
    }

    // private async Task SeedDataAsync()
    // {
    //     // Add initial data seeding logic here if necessary
    // }
}