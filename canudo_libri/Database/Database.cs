using System.Runtime.InteropServices.JavaScript;
using Npgsql;
using NpgsqlTypes;

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
        const string query = "SELECT COUNT(*) FROM users WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        await cmd.PrepareAsync();
        var count = (long)await cmd.ExecuteScalarAsync();
        return count;
    }

    public static async Task<int> AddUser(NpgsqlConnection db, User user)
    {
        const string query = "INSERT INTO users(user_id, first_name, last_name, username, lang, status, rank, banned) VALUES(@user_id, @first_name, @last_name, @username, @lang, @status, @rank, @banned) ON CONFLICT (user_id) DO UPDATE SET first_name = @first_name, last_name = @last_name, username = @username";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", user.Id);
        cmd.Parameters.AddWithValue("first_name", user.FirstName);
        cmd.Parameters.AddWithValue("last_name", user.LastName ?? "");
        cmd.Parameters.AddWithValue("username", user.Username ?? "");
        cmd.Parameters.AddWithValue("lang", user.LanguageCode ?? "");
        cmd.Parameters.AddWithValue("status", "");
        cmd.Parameters.AddWithValue("rank", "1");
        cmd.Parameters.AddWithValue("banned", false);
        await cmd.PrepareAsync();
        var r = await cmd.ExecuteNonQueryAsync();
        return r;
    }

    public static async Task<int> GetSubs(NpgsqlConnection db)
    {
        const string query = "SELECT COUNT(*) FROM users";
        var cmd = new NpgsqlCommand(query, db);
        var count = (int)await cmd.ExecuteScalarAsync();
        return count;
    }

    public static async Task<List<DBUser>> GetUsers(NpgsqlConnection db)
    {
        const string query = "SELECT * FROM users";
        var cmd = new NpgsqlCommand(query, db);
        var reader = await cmd.ExecuteReaderAsync();
        List<DBUser> results = [];
        while (await reader.ReadAsync())
        {
            var element = new DBUser()
            {
                FirstName = reader.GetString(0),
                LastName = reader.GetString(1),
                Username = reader.GetString(2),
                Lang = reader.GetString(3),
                Status = reader.GetString(4),
                Rank = reader.GetInt32(5),
                Banned = reader.GetBoolean(6),
            };
            results.Add(element);
        }

        return results;
    }

    public static async Task<DBUser> GetUser(NpgsqlConnection db, long userId)
    {
        const string query = "SELECT * FROM users WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        await cmd.PrepareAsync();
        var dr = await cmd.ExecuteReaderAsync();
        
        if (dr.Read() == false)
        {
            await dr.CloseAsync();
            return [];
        }

        var user = new DBUser() { 
            FirstName = (string)dr[0], 
            LastName = (string)dr[1]),
            Username = (string)dr[2],
            Lang = (string)dr[3],
            Status = (string)dr[4],
            Rank = (int)dr[5],
            Banned = (bool)dr[6]
        };
        await dr.CloseAsync();
        return user;
    }

}