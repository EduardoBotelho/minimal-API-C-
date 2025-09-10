using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;

namespace minimal_api.infraestrutura.Intefaces
{
    public interface IVeiculosServico
    {
        //List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo BuscarPorId(int id);
        Veiculo Incluir(Veiculo veiculo);
        void  Atualizar(Veiculo veiculo);
        void  Apagar(Veiculo veiculo);    

        IEnumerable<Veiculo> Todos(int pagina, string? busca, string? ordem);
        IEnumerable<Veiculo> ObterTodos();
    }
}