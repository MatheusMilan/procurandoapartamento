using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain.Services.Interfaces;
using ProcurandoApartamento.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using LanguageExt;
using System.Collections.Generic;
using LanguageExt.ClassInstances;

namespace ProcurandoApartamento.Domain.Services
{
    public class ApartamentoService : IApartamentoService
    {
        protected readonly IApartamentoRepository _apartamentoRepository;

        public ApartamentoService(IApartamentoRepository apartamentoRepository)
        {
            _apartamentoRepository = apartamentoRepository;
        }

        public virtual async Task<Apartamento> Save(Apartamento apartamento)
        {
            await _apartamentoRepository.CreateOrUpdateAsync(apartamento);
            await _apartamentoRepository.SaveChangesAsync();
            return apartamento;
        }

        public virtual async Task<IPage<Apartamento>> FindAll(IPageable pageable)
        {
            var page = await _apartamentoRepository.QueryHelper()
                .GetPageAsync(pageable);
            return page;
        }

        public virtual async Task<Apartamento> FindOne(long id)
        {
            var result = await _apartamentoRepository.QueryHelper()
                .GetOneAsync(apartamento => apartamento.Id == id);
            return result;
        }

        public virtual async Task Delete(long id)
        {
            await _apartamentoRepository.DeleteByIdAsync(id);
            await _apartamentoRepository.SaveChangesAsync();
        }
        internal class Selecao
        {
            public Selecao(int quadra, string estabelecimento)
            {
                Quadra = quadra;
                Estabelecimento = estabelecimento;
            }

            public int Quadra { get; set; }
            public string Estabelecimento { get; set; }

        }

        public string MelhorApartamento(string[] pEstabelecimentos)
        {
            var lst = _apartamentoRepository.GetAllAsync();
            List<Selecao> opcoes = new List<Selecao>();
            if (lst.Result.Count() > 0)
            {
                List<Apartamento> lstAp = lst.Result.ToList();
                foreach (var item in lstAp)
                {
                    if (item.EstabelecimentoExiste == true &&
                        pEstabelecimentos[0].Split(",").Contains(item.Estabelecimento) &&
                        item.ApartamentoDisponivel == true)
                        opcoes.Add(new Selecao(item.Quadra, item.Estabelecimento));
                }
                if (pEstabelecimentos[0].Split(",").Length == 1)
                    return "Quadra " + opcoes.Max(p => p.Quadra).ToString();
                else
                {
                    var quadra = -1;
                    var contEstabelcimentos = -1;
                    var lstGrupQuadras = opcoes.GroupBy(p => p.Quadra).ToList();
                    foreach (var item in lstGrupQuadras)
                    {
                        if (contEstabelcimentos < item.Count() || (quadra < item.Key && contEstabelcimentos == item.Count()))
                        {
                            quadra = item.Key;
                            contEstabelcimentos = item.Count();
                        }
                    }
                    return "Quadra " + quadra;
                }
            }
            else
                return "Base de dados Vazia";
        }
    }
}
