using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infnet.PesqMgm.Domain.Pesquisas;
using Infnet.PesqMgm.Domain.Repositories;
using Infnet.PesqMgm.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infnet.PesqMgm.Infrastructure.Data.Repositories;

public class SqlServerPesquisaRepository : IPesquisaRepository
{
    private readonly PesquisaDbContext _context;

    public SqlServerPesquisaRepository(PesquisaDbContext context)
    {
        _context = context;
    }

    public async Task Add(Pesquisa pesquisa)
    {
        await _context.Pesquisas.AddAsync(pesquisa);
    }

    public Task Update(Pesquisa pesquisa)
    {
        _context.Pesquisas.Update(pesquisa);
        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        var pesquisa = await GetById(id);
        if (pesquisa != null)
        {
            _context.Pesquisas.Remove(pesquisa);
        }
    }

    public async Task<Pesquisa> GetById(Guid id)
    {
        return await _context.Pesquisas
            .Include(p => p.Gestor)
            .FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id);
    }

    public async Task<List<Pesquisa>> GetAll()
    {
        return await _context.Pesquisas
            .Include(p => p.Gestor)
            .ToListAsync();
    }

    public async Task<bool> Exists(Guid id)
    {
        return await _context.Pesquisas
            .AnyAsync(p => EF.Property<Guid>(p, "Id") == id);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}