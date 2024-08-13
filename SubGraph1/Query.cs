namespace SubGraph1;

public record Department(string Id, string Name);

public class DepartmentBatchLoader(IBatchScheduler batchScheduler, DataLoaderOptions? options = null)
    : BatchDataLoader<string, Department>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<string, Department>> LoadBatchAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyDictionary<string, Department>>(new Dictionary<string, Department>
        {
            ["oh:department:FEB"] = new Department("oh:department:FEB", "FEB")
        });
    }
}

[InterfaceType("Programme")]
public interface IProgramme
{
    string Id { get; set; }
    string DepartmentId { get; set; }

    Task<Department> GetDepartment([Parent] IProgramme programme, DepartmentBatchLoader loader);
}

public class DegreeProgramme : IProgramme
{
    public required string Id { get; set; }
    public required string DepartmentId { get; set; }

    public Task<Department> GetDepartment([Parent] IProgramme programme, DepartmentBatchLoader loader) => loader.LoadAsync(programme.DepartmentId);

    public required double Credits { get; set; } 
}

public class AdministrativeProgramme : IProgramme
{
    public required string Id { get; set; }
    public required string DepartmentId { get; set; }
    public Task<Department> GetDepartment([Parent] IProgramme programme, DepartmentBatchLoader loader) => loader.LoadAsync(programme.DepartmentId);
}

public class Query
{
    private readonly List<Department> _departments =
    [
        new Department("oh:department:FEB", "FEB")
    ];

    private readonly List<IProgramme> _programmes =
    [
        new DegreeProgramme {Id = "oh:programme:EMA_ACC", DepartmentId = "oh:department:FEB", Credits = 60},
        new DegreeProgramme {Id = "oh:programme:EBA_ACT", DepartmentId = "oh:department:FEB", Credits = 180},
        new AdministrativeProgramme {Id = "oh:programme:EOV_B_ADM", DepartmentId = "oh:department:FEB"}
    ];
    
    public Task<Department?> GetDepartmentById(string id)
    {
        return Task.FromResult(_departments.SingleOrDefault(d => d.Id == id));
    }

    public Task<IProgramme[]> GetProgrammes()
    {
        var programmes = _programmes.Where(p => p.DepartmentId == "oh:department:FEB");
        return Task.FromResult(programmes.ToArray());
    }
}