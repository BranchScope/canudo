using System.Collections;

namespace canudo_libri.Database;

public class DBUser
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Lang { get; set; }
    public string Status { get; set; }
    public int Rank { get; set; }
    public bool Banned { get; set; }
}

public class DBSubject
{
    public string CodeName { get; set; }
    public string Name { get; set; }
}

public class DBAdvertisement
{
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Book { get; set; }
    public string BookCode { get; set; }
    public List<int> Years { get; set; }
    public List<string> Contacts { get; set; }
    public long FromUser { get; set; }
}