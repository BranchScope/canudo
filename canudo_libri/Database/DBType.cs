using System.Collections;

namespace canudo_libri.Database;

public class DBUser : IEnumerable
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Lang { get; set; }
    public string Status { get; set; }
    public int Rank { get; set; }
    public bool Banned { get; set; }
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}