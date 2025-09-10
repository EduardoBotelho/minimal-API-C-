using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.infraestrutura.Intefaces;

namespace minimal_api.Dominio.Servicos
{
    public class VeiculoServico : IVeiculosServico
    {
        private readonly DbContext _contexto;
        public VeiculoServico(DbContext contexto)
        {
            _contexto = contexto;
        }

        public void Apagar(Veiculo veiculo)
        {
            _contexto.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _contexto.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo BuscarPorId(int id)
        {
            var veiculo = _contexto.Set<Veiculo>().Find(id);

            return _contexto.Set<Veiculo>().Find(id);
        }

        public Veiculo Incluir(Veiculo veiculo)
        {
            _contexto.Add(veiculo);
            _contexto.SaveChanges();
            return veiculo;
        }
        public IEnumerable<Veiculo> ObterTodos()
        {
            return _contexto.Set<Veiculo>().ToList();
        }
    public IEnumerable<Veiculo> Todos(int pagina, string? busca, string? ordem)
    {
        // Aqui vai a sua lógica para buscar, filtrar e ordenar os veículos.
        // Este é apenas um exemplo.
        throw new NotImplementedException(); // Comece com isso e depois implemente a lógica.
    }

       /* public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
          {

              var query =  _contexto.Veiculos.AsQueryable();
              if(!string.IsNullOrEmpty(nome))
              {
                  query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(),$"%{nome}%"));
              }
          var itensporpagina = 10;
              query = query.Skip((pagina - itensporpagina) * 10).Take(10);
              return query.toList();*/
    }
    }
