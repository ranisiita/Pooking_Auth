namespace Microservicio.Pooking.Auth.DataAccess.Common;

/// <summary>
/// Envuelve una página de resultados junto con los metadatos de paginación.
/// Es el tipo de retorno estándar de cualquier consulta paginada en la DAL.
/// </summary>
/// <typeparam name="T">Tipo de la entidad o proyección devuelta.</typeparam>
public class PagedResult<T>
{
    // -------------------------------------------------------------------------
    // Datos
    // -------------------------------------------------------------------------

    /// <summary>
    /// Elementos de la página actual.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; }

    // -------------------------------------------------------------------------
    // Metadatos de paginación
    // -------------------------------------------------------------------------

    /// <summary>
    /// Número de página actual (base 1).
    /// </summary>
    public int PaginaActual { get; init; }

    /// <summary>
    /// Cantidad máxima de elementos por página.
    /// </summary>
    public int TamanoPagina { get; init; }

    /// <summary>
    /// Total de registros que existen en la fuente sin paginar.
    /// </summary>
    public int TotalRegistros { get; init; }

    /// <summary>
    /// Total de páginas calculado a partir de TotalRegistros y TamanoPagina.
    /// </summary>
    public int TotalPaginas => TamanoPagina > 0
        ? (int)Math.Ceiling((double)TotalRegistros / TamanoPagina)
        : 0;

    // -------------------------------------------------------------------------
    // Flags de navegación — útiles en la capa de presentación / API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Indica si existe una página anterior a la actual.
    /// </summary>
    public bool TienePaginaAnterior => PaginaActual > 1;

    /// <summary>
    /// Indica si existe una página siguiente a la actual.
    /// </summary>
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

    // -------------------------------------------------------------------------
    // Constructor principal
    // -------------------------------------------------------------------------

    /// <summary>
    /// Crea un resultado paginado con todos sus metadatos.
    /// </summary>
    /// <param name="items">Elementos de la página actual.</param>
    /// <param name="totalRegistros">Total de registros sin paginar.</param>
    /// <param name="paginaActual">Número de página solicitada (base 1).</param>
    /// <param name="tamanoPagina">Cantidad de elementos por página.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Si paginaActual es menor que 1 o tamanoPagina es menor que 1.
    /// </exception>
    public PagedResult(
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
    }

    // -------------------------------------------------------------------------
    // Factory — página vacía (útil cuando la consulta no retorna resultados)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Crea un resultado paginado vacío manteniendo los metadatos de la solicitud.
    /// Evita retornar null desde los repositorios cuando no hay datos.
    /// </summary>
    public static PagedResult<T> Vacio(int paginaActual, int tamanoPagina) =>
        new([], totalRegistros: 0, paginaActual, tamanoPagina);
}