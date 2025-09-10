using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace infraestrutura.DB;

public class DbContexto : DbContext
{
        private readonly IConfiguration _configurationAppSettings;
    public DbContexto(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
              new Administrador
              {
                  Id = 1,
                  Perfil = "Adm",
                  Email = "administrador@teste.com",
                  Senha = "12345678"
              }
        );
           }
        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

            var stringConexao = _configurationAppSettings.GetConnectionString("mysql");
            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(
                    stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                 );
            }
            else
            {
                optionsBuilder.UseMySql("string de conexao",
                ServerVersion.AutoDetect("string de conexao")
                 );
            }
        }
    }
        

}