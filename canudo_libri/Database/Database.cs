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
        const string query = "SELECT COUNT(*) FROM users WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
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
        cmd.Parameters.AddWithValue("rank", 1);
        cmd.Parameters.AddWithValue("banned", false);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }

    public static async Task<long> GetSubs(NpgsqlConnection db)
    {
        const string query = "SELECT COUNT(*) FROM users";
        var cmd = new NpgsqlCommand(query, db);
        var count = (long)await cmd.ExecuteScalarAsync();
        
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
                UserId = reader.GetInt64(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Username = reader.GetString(3),
                Lang = reader.GetString(4),
                Status = reader.GetString(5),
                Rank = reader.GetInt32(6),
                Banned = reader.GetBoolean(7),
            };
            results.Add(element);
        }
        await reader.CloseAsync();
        
        return results;
    }

    public static async Task<DBUser> GetUser(NpgsqlConnection db, long userId)
    {
        const string query = "SELECT * FROM users WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        var reader = await cmd.ExecuteReaderAsync();
        var user = new DBUser();
        while (await reader.ReadAsync())
        {
            user = new DBUser()
            { 
                UserId = reader.GetInt64(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Username = reader.GetString(3),
                Lang = reader.GetString(4),
                Status = reader.GetString(5),
                Rank = reader.GetInt32(6),
                Banned = reader.GetBoolean(7)
            };
        }
        await reader.CloseAsync();
        
        return user;
    }

    public static async Task<int> SetStatus(NpgsqlConnection db, long userId, string status)
    {
        const string query = "UPDATE users SET status = @status WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("status", status);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }
    
    public static async Task<int> SetLang(NpgsqlConnection db, long userId, string lang)
    {
        const string query = "UPDATE users SET lang = @lang WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("lang", lang);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }
    
    public static async Task<int> SetRank(NpgsqlConnection db, long userId, int rank)
    {
        const string query = "UPDATE users SET rank = @rank WHERE user_id = @user_id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("rank", rank);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }
    
    // here comes the fun, turuturu
    public static async Task<List<DBSubject>> GetSubjects(NpgsqlConnection db)
    {
        const string query = "SELECT * FROM subjects";
        var cmd = new NpgsqlCommand(query, db);
        var reader = await cmd.ExecuteReaderAsync();
        List<DBSubject> results = [];
        while (await reader.ReadAsync())
        {
            var element = new DBSubject()
            {
                CodeName = reader.GetString(0),
                Name = reader.GetString(1)
            };
            results.Add(element);
        }
        await reader.CloseAsync();
        
        return results;
    }
    
    public static async Task<DBSubject> GetSubject(NpgsqlConnection db, string codeName)
    {
        const string query = "SELECT * FROM subjects WHERE code_name = @code_name";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("code_name", codeName);
        var reader = await cmd.ExecuteReaderAsync();
        var subject = new DBSubject();
        while (await reader.ReadAsync())
        {
            subject = new DBSubject()
            { 
                CodeName = reader.GetString(0),
                Name = reader.GetString(1)
            };
        }
        await reader.CloseAsync();
        
        return subject;
    }

    public static async Task<int> AddSubject(NpgsqlConnection db, string codeName, string name)
    {
        const string query = "INSERT INTO subjects(code_name, name) VALUES(@code_name, @name)";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("code_name", codeName);
        cmd.Parameters.AddWithValue("name", name);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }
    
    public static async Task<int> DeleteSubject(NpgsqlConnection db, string codeName)
    {
        const string query = "DELETE FROM subjects WHERE code_name = @code_name";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("code_name", codeName);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }

    public static async Task<DBAdvertisement> GetAdvertisementById(NpgsqlConnection db, int id)
    {
        const string query = "SELECT * FROM ads WHERE id = @id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("id", id);
        var reader = await cmd.ExecuteReaderAsync();
        var ad = new DBAdvertisement();
        while (await reader.ReadAsync())
        {
            ad = new DBAdvertisement()
            { 
                Id = reader.GetInt32(0),
                Subject = reader.IsDBNull(1) ? null : reader.GetString(1),
                Book = reader.IsDBNull(2) ? null : reader.GetString(2),
                BookCode = reader.IsDBNull(3) ? null : reader.GetString(3),
                Years = reader.IsDBNull(4) ? null : reader.GetFieldValue<List<int>>(4),
                Contacts = reader.IsDBNull(5) ? null : ConvertToJaggedList(reader.GetFieldValue<string[,]>(5)),
                FromUser = reader.GetInt64(6)
            };
        }
        await reader.CloseAsync();
        
        return ad;
    }
    
    // a little util
    static List<List<string>> ConvertToJaggedList(string[,] multidimensionalArray)
    {
        int outerLength = multidimensionalArray.GetLength(0);
        int innerLength = multidimensionalArray.GetLength(1);
        var jaggedList = new List<List<string>>(outerLength);

        for (int i = 0; i < outerLength; i++)
        {
            var innerList = new List<string>(innerLength);
            for (int j = 0; j < innerLength; j++)
            {
                innerList.Add(multidimensionalArray[i, j]);
            }
            jaggedList.Add(innerList);
        }

        return jaggedList;
    }

    public static async Task<int> CreateAdvertisement(NpgsqlConnection db, long fromUser)
    {
        const string query = "INSERT INTO ads(from_user) VALUES(@from_user) RETURNING id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("from_user", fromUser);
        var id = (int)await cmd.ExecuteScalarAsync();
        
        return id;
    }

    public static async Task<int> UpdateAdvertisement(NpgsqlConnection db, int id, string whateverItTakes, object value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var query = $"UPDATE ads SET {whateverItTakes} = @value WHERE id = @id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("value", value);
        cmd.Parameters.AddWithValue("id", id);
        var r = await cmd.ExecuteNonQueryAsync();
        
        return r;
    }

    public static async Task<int> DeleteAdvertisement(NpgsqlConnection db, int id)
    {
        const string query = "DELETE FROM ads WHERE id = @id";
        var cmd = new NpgsqlCommand(query, db);
        cmd.Parameters.AddWithValue("id", id);
        var r = await cmd.ExecuteNonQueryAsync();

        return r;
    }
    
}
