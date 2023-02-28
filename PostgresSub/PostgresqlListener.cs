using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostgresSub;

public class PostgresqlListener
{
    private readonly ILogger logger;
    private readonly NpgsqlConnection connection;

    public PostgresqlListener(IConfiguration configuration, ILogger<PostgresqlListener> logger)
    {
        var connectionString = configuration.GetConnectionString("Postgresql");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        this.connection = new NpgsqlConnection(connectionString);
        this.logger = logger;
    }

    public async Task Listen()
    {
        logger.LogInformation("Starting to listen to channel 'message'");

        await connection.OpenAsync();
        connection.Notification += ConnectionOnNotification;


        var cmd = new NpgsqlCommand("listen message;", connection);
        cmd.ExecuteNonQuery();

        while (true)
        {
            await connection.WaitAsync();
        }
    }

    private void ConnectionOnNotification(object sender, NpgsqlNotificationEventArgs e)
    {
        logger.LogInformation("Notification received. Channel: {Channel}; Payload: {Payload}", e.Channel, e.Payload);
    }
}