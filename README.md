#Postgres Pub/Sub

Code sample to use Postgresql Notify and Listen commands as a Pub/Sub server, with C# and Npgsql.

## Try it

You can start a Postgresql server with the following docker command :

	docker run -d -p 5432:5432 --name $name -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=$password postgres:latest

You can now start the two console apps. One will send a message, the second will display the message received.

## How it works : send a notification

You can send a notification to a specific channel with either of the following command :

	NOTIFY channel, 'message';
	SELECT pg_notify('channel', 'message');

In a C# app with Npgsql, you can do it like that :

    await using var dataSource = NpgsqlDataSource.Create(connectionString);
    await using (var cmd = dataSource.CreateCommand($"SELECT pg_notify('message',  @message)"))
    {
        cmd.Parameters.AddWithValue("message", msg);
        await cmd.ExecuteNonQueryAsync();
    }


## How it works : listen to a channel

You can listen to a channel with this command :

    LISTEN channel

In a C# app, you can do it like that :

First, you open the connection, and you register the Notification event :

    await connection.OpenAsync();
    connection.Notification += (sender, args) =>
    {
        logger.LogInformation("Notification received. Channel: {Channel}; Payload: {Payload}", args.Channel, args.Payload);
    };

Now you can start listening the channel :

	var cmd = dataSource.CreateCommand("LISTEN channel", connection);	
	await cmd.ExecuteNonQueryAsync();
