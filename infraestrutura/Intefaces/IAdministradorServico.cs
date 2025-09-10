using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servico;

namespace minimal_api.infraestrutura.Intefaces
{
    public interface IAdministradorServico
    {
        
        Administrador Login(string email, LoginDTO loginDTO);
        Administrador Incluir(Administrador administradorDTO);
        Administrador? Todos(int? id);
        Administrador BuscarPorId(int id);
        object CriarAdministrador(LoginDTO loginDTO);
        object Login(LoginDTO loginDTO);
    }
}