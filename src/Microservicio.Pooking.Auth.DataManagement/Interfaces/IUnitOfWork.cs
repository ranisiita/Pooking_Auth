using Microservicio.Pooking.Auth.DataAccess.Queries;
using Microservicio.Pooking.Auth.DataAccess.Repositories.Interfaces;
namespace Microservicio.Pooking.Auth.DataManagement.Interfaces;

/// <summary>
/// Unidad de trabajo: centraliza repositorios y el guardado transaccional.
/// La capa de negocio nunca llama SaveChanges directamente;
/// siempre lo hace a través de este contrato.W
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Usuarios
    IUsuarioRepository UsuarioRepository { get; }
    IRolRepository RolRepository { get; }
    UsuarioQueryRepository UsuarioQueryRepository { get; }

    // Persistencia
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}