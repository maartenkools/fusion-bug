namespace SubGraph2;

public record Department(string Id, string DisplayName);

public class Query
{
    private readonly List<Department> _departments =
    [
        new Department("oh:department:FEB", "Faculteit Economie en Bedrijfskunde")
    ];
    
    public Task<Department?> GetDepartmentById(string id)
    {
        return Task.FromResult(_departments.SingleOrDefault(d => d.Id == id));
    }
}