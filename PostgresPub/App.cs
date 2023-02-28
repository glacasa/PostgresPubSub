

using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace PostgresPub;

public class App
{
    private readonly string connectionString;

    public App(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Postgresql")!;
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
    }

    public async Task Run()
    {
        Console.WriteLine("Write your message and press Enter to send.");

        while (true)
        {
            var msg = Console.ReadLine();
            await Notify(msg);
        }
    }

    private async Task Notify(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return;

        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using (var cmd = dataSource.CreateCommand($"SELECT pg_notify('message',  @message)"))
        {
            cmd.Parameters.AddWithValue("message", msg);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
