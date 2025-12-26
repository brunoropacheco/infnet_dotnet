using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infnet.PesqMgm.Domain.Pesquisas;

namespace Infnet.PesqMgm.Domain.Repositories;

public interface IUsuarioRepository
{
    Task Add(Usuario usuario);
    
    Task Update(Usuario usuario);
    
    Task Delete(Guid id);
    
    Task<Usuario?> GetById(Guid id);
    
    Task<List<Usuario>> GetAll();
    
    Task<bool> Exists(Guid id);

    Task<int> SaveChangesAsync();
}