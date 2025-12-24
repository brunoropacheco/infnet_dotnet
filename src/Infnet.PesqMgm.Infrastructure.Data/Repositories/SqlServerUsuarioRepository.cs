using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infnet.PesqMgm.Domain.Pesquisas;
using Infnet.PesqMgm.Domain.Repositories;
using Infnet.PesqMgm.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infnet.PesqMgm.Infrastructure.Data.Repositories;

public class SqlServerUsuarioRepository : IUsuarioRepository
{
    private readonly PesquisaDbContext _context;

    public SqlServerUsuarioRepository(PesquisaDbContext context)
    {
        _context = context;
    }

    public async Task Add(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public Task Update(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        var usuario = await GetById(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
        }
    }

    public async Task<Usuario> GetById(Guid id)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => EF.Property<Guid>(u, "Id") == id);
    }

    public async Task<List<Usuario>> GetAll()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<bool> Exists(Guid id)
    {
        return await _context.Usuarios
            .AnyAsync(u => EF.Property<Guid>(u, "Id") == id);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}