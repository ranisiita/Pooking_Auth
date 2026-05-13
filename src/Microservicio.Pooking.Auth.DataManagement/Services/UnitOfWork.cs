using Microservicio.Pooking.Auth.DataAccess.Context;
using Microservicio.Pooking.Auth.DataAccess.Queries;
using Microservicio.Pooking.Auth.DataAccess.Repositories;
using Microservicio.Pooking.Auth.DataAccess.Repositories.Interfaces;
using Microservicio.Pooking.Auth.DataManagement.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;

    // Usuarios
    public IUsuarioRepository UsuarioRepository { get; }
    public IRolRepository RolRepository { get; }

    // Queries (solo lectura — AsNoTracking)
    public UsuarioQueryRepository UsuarioQueryRepository { get; }
    public UnitOfWork(AuthDbContext context)
    {
        _context = context;

        UsuarioRepository = new UsuarioRepository(_context);
        RolRepository = new RolRepository(_context);
        
        UsuarioQueryRepository = new UsuarioQueryRepository(_context);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}