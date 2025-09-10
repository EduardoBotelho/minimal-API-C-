using System;
using infraestrutura.DB;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.infraestrutura.Intefaces;
using minimal_api.Migrations;
namespace minimal_api.Dominio.Servico
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }

        object IAdministradorServico.Login(LoginDTO loginDTO)
        {
            return Login(loginDTO);
        }
        public Administrador Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
            return administrador;
        }
        public Administrador? BuscarPorId(int id)
        {
            return _contexto.Administradores.Where(v => v.id == id).FirstOrDefault();
        }


        public List<Administrador> Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();
            int itensPorPagina = 10;
            if (pagina != null)
                //query = query.Skip({ (int)pagina - 1} *itensPorPagina).Take(itensPorPagina);
        }
    }

        
}