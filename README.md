[![Build Status](https://jadsena.visualstudio.com/Redis.Data/_apis/build/status/jadsena.Redis.Data?branchName=main&jobName=Job)](https://jadsena.visualstudio.com/Redis.Data/_build/latest?definitionId=9&branchName=main)
![Nuget](https://img.shields.io/nuget/dt/Redis.Data?style=plastic)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Redis.Data?style=plastic)

# Redis.Data

Componente para facilitar a utilização do Redis como banco de dados.

## Instalação

Abrir o gerenciador de pacotes nuget e pesquisar na aba `Procurar` pelo nome do pacote `Redis.Data` e então clique em instalar.



## Utilização

Na class `startup.cs` realizar as seguintes alterações:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddRedisDB(Configuration.GetConnectionString("Redis"));

    // Register the Swagger generator, defining 1 or more Swagger documents
    services.AddSwaggerGen();
}
```

Configurando a conexão com o Redis no arquivo `appSettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "Redis": "127.0.0.1:6379"
  }
}
```

Usando a aconexão em um Controller

```csharp
[Route("api/[controller]")]
[ApiController]
public class RegraController : ControllerBase
{
    private readonly IRedisContext _context;

    public RegraController(IRedisContext context)
    {
        _context = context;
    }
    // GET: api/<RegraController>
    [HttpGet]
    public async Task<ModeloDadosCollection> Get()
    {
        return new ModeloDadosCollection(await _context.GetAllAsync());
    }

    // GET api/<RegraController>/5
    [HttpGet("{key}")]
    public async Task<ModeloDados> Get(string key)
    {
        return await _context.GetAsync<ModeloDados>(key);
    }

    // POST api/<RegraController>
    [HttpPost]
    public async Task Post([FromBody] ModeloDados value)
    {
        if (!ModelState.IsValid) return;
        await _context.SetAsync(value.Nome, JsonSerializer.Serialize(value));
    }

    // PUT api/<RegraController>/5
    [HttpPut("{key}")]
    public async Task Put(string key, [FromBody] ModeloDados value)
    {
        if (!ModelState.IsValid) return;
        await _context.SetAsync(key, JsonSerializer.Serialize(value));
    }

    // DELETE api/<RegraController>/5
    [HttpDelete("{key}")]
    public async Task Delete(string key)
    {
        await _context.DeleteAsync(key);
    }
}
```