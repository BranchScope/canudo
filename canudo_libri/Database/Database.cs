using Npgsql;

namespace canudo_libri.Database;

public class Database
{
    internal NpgsqlConnection Db { get; set; }
    private static readonly string? IsRunningInContainer = Environment.GetEnvironmentVariable("CONTAINERIZE_THESE_NUTS");
    private static readonly string? PgHost = IsRunningInContainer == "true" ? "database" : "localhost"; //200iq stuffs here
    private static readonly string? PgUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
    private static readonly string? PgPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    private static readonly string? PgDatabaseName = Environment.GetEnvironmentVariable("POSTGRES_DBNAME");

    private static readonly string PgConnectionString = $"Host={PgHost};Username={PgUsername};Password={PgPassword};Database={PgDatabaseName}";

    public Database()
    {
        var con = new NpgsqlConnection(connectionString: PgConnectionString);
        con.Open();
        this.Db = con;
    }

    public static async Task<long> CheckUser(NpgsqlConnection db, long userId)
    {
        var query = "SELECT COUNT(*) FROM users WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        await cmd.PrepareAsync();
        var count = (long)await cmd.ExecuteScalarAsync();
        return count;
    }

    public static async Task<int> AddUser(NpgsqlConnection db, User user)
    {
        var query =
            "INSERT INTO users(user_id, first_name, last_name, username, lang) VALUES(@user_id, @first_name, @last_name, @username, @lang) ON CONFLICT (user_id) DO UPDATE SET first_name = @first_name, last_name = @last_name, username = @username";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", user.Id);
        cmd.Parameters.AddWithValue("first_name", user.FirstName);
        cmd.Parameters.AddWithValue("last_name", user.LastName ?? "");
        cmd.Parameters.AddWithValue("username", user.Username ?? "");
        cmd.Parameters.AddWithValue("lang", user.LanguageCode ?? "");
        await cmd.PrepareAsync();
        var r = await cmd.ExecuteNonQueryAsync();
        return r;
    }

}