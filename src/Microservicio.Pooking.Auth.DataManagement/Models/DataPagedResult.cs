using Microservicio.Pooking.Auth.DataAccess.Common;

namespace Microservicio.Pooking.Auth.DataManagement.Models;

/// <summary>
/// Resultado paginado de la capa DataManagement, desacoplado de <see cref="PagedResult{T}"/> de DataAccess.
/// </summary>
public sealed class DataPagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int PaginaActual { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
    public bool TienePaginaAnterior { get; init; }
    public bool TienePaginaSiguiente { get; init; }

    public DataPagedResult(
        IEnumerable<T> items,
        int totalRegistros,
        int paginaActual,
        int tamanoPagina)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(paginaActual, 1, nameof(paginaActual));
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanoPagina, 1, nameof(tamanoPagina));
        ArgumentOutOfRangeException.ThrowIfNegative(totalRegistros, nameof(totalRegistros));

        Items = items.ToList().AsReadOnly();
        TotalRegistros = totalRegistros;
        PaginaActual = paginaActual;
        TamanoPagina = tamanoPagina;
        TotalPaginas = tamanoPagina > 0
            ? (int)Math.Ceiling((double)totalRegistros / tamanoPagina)
            : 0;
        TienePaginaAnterior = PaginaActual > 1;
        TienePaginaSiguiente = PaginaActual < TotalPaginas;
    }

    /// <summary>
    /// Mapea un <see cref="PagedResult{T}"/> de DataAccess al modelo de esta capa.
    /// </summary>
    public static DataPagedResult<T> DesdeDal(PagedResult<T> origen) =>
        new(origen.Items, origen.TotalRegistros, origen.PaginaActual, origen.TamanoPagina);

    /// <summary>
    /// Mapea un resultado paginado de DataAccess transformando cada ítem.
    /// </summary>
    public static DataPagedResult<TDestino> DesdeDal<TOrigen, TDestino>(
        PagedResult<TOrigen> origen,
        Func<TOrigen, TDestino> mapearItem) =>
        new(
            origen.Items.Select(mapearItem),
            origen.TotalRegistros,
            origen.PaginaActual,
            origen.TamanoPagina);

    public static DataPagedResult<T> Vacio(int paginaActual, int tamanoPagina) =>
        new(Array.Empty<T>(), totalRegistros: 0, paginaActual, tamanoPagina);
}
