using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servico;
using minimal_api.Dominio.Servicos;
using minimal_api.Enum;
using minimal_api.infraestrutura.Intefaces;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region Home
app.MapGet("/", () => Results.Json("new Home")).WithTags("Home");
#endregion


#region Builder
var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>{
    option.TokenValidationParameters = new TokenValidationParameters {;

    ValidateLifetime = true,
    //ValidateAudience = JwtSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    ValidateAudience = false,
    ValidateIssuer = false

    };

});

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculosServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Jwt",
        In = ParameterLocation.Header,
        Description = "Insira o token Jwt aqui:{seu token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {

        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        });
});

builder.Services.AddDbContext<infraestrutura.DB.DbContexto>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))));

builder.Services.AddAuthorization();

#endregion


#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    if (!string.IsNullOrEmpty(key))
        return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>();
    {
        new Claim("Email", administrador.Email),
        new Claim("perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil),
    };

    var token = new JwtSecurityToken{
        claims:claims,
        expires: Datetime.now.AddDay(1);
        SigningCredentials: credentials
        
    };

    
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);
    if (adm != null)
    { string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }else    
        return Results.Unauthorized();
}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
var adms = new List<AdmiministradorModelView>();
var administradores = administradorServico.Todos(pagina);
foreach (var adm in administradores)
{
    adms.Add(new AdmiministradorModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = (Perfil)Enum.Parse(typeof(Perfil), adm.Perfil)
    });
    }
    return Results.Ok(adms);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"})
.WithTags("Administradores");

app.MapGet("/Administradores/{id}", ([FromRoute]int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);
    if (administrador == null) return Results.NotFound();
    return Results.Ok(new AdmiministradorModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
    });   
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"})
.WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico, AdministradorDTO administradorDTO) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email nao pode ser vazio");
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha nao pode ser vazio");
    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("Perfil nao pode ser vazio");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);
    if (administradorDTO.Perfil != null)
    {
        var administrador = new Administrador
        {

            Email = administrador.Email,
            Senha = administrador.Senha,
            Perfil = administrador.Perfil
        };
        administradorServico.Incluir(administrador);
    }
    

}).RequireAuthorization().WithTags("Administradores");
#endregion


#region Veiculos

 ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    
    var validacao = new ErrosDeValidacao{
        Mensagens  = new List<string>()
    };

    validacao.Mensagens = new List<string>();
    if(string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagens.Add("O nome do veiculo é obrigatório.");
    }
    if(string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagens.Add("A marca do veiculo é obrigatória.");
    }
    if(veiculoDTO.Ano < 1950)
    {
        validacao.Mensagens.Add("Veiculos anteriores a 1950 não são aceitos.");
    }
    return validacao;
    
}

app.MapPost("/Veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculosServico.Incluir(veiculo);
    return Results.Created(
    $"/veiculos/{veiculo.Id}",
    veiculo);

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm,Editor"})
.WithTags("Veiculos");

app.MapGet("/Veiculos", ([FromQuery] int pagina, string? busca, string? ordem, IVeiculosServico veiculosServico) =>
{
    var veiculos = veiculosServico.Todos(pagina, busca, ordem);
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

//Buscar veiculo por id
app.MapGet("/Veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) =>
{
    var veiculo = veiculosServico.BuscarPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"})
.WithTags("Veiculos");
//Alterar um veiculo
app.MapPut("/Veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
{

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    var veiculo = veiculosServico.BuscarPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;
    veiculosServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"})
.WithTags("Veiculos");
//Deletar um veiculo
app.MapDelete("/Veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) =>
{
    var veiculo = veiculosServico.BuscarPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    veiculosServico.Apagar(veiculo); 
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{ Roles = "Adm"})
.WithTags("Veiculos");

#endregion 

#region App

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.Urls.Add("http://localhost:5000");
app.Urls.Add("https://localhost:5001");
// ...existing code...
#endregion
app.Run();

