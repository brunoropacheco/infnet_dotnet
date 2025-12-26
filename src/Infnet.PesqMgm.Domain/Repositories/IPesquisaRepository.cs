using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infnet.PesqMgm.Domain.Pesquisas;

namespace Infnet.PesqMgm.Domain.Repositories;

public interface IPesquisaRepository
{
    Task Add(Pesquisa pesquisa);
    
    Task Update(Pesquisa pesquisa);
    
    Task Delete(Guid id);
    
    Task<Pesquisa?> GetById(Guid id);
    
    Task<List<Pesquisa>> GetAll();
    
    Task<bool> Exists(Guid id);

    Task<int> SaveChangesAsync();
}