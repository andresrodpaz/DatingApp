namespace API.Helpers;
public class PaginationParams
{
    /// <summary>
    /// El tamaño máximo permitido para una página.
    /// </summary>
    private const int MaxPageSize = 50;

    /// <summary>
    /// Obtiene o establece el número de la página actual. El valor predeterminado es 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// El tamaño de la página. Valor predeterminado es 10.
    /// </summary>
    private int _pageSize = 10;

    /// <summary>
    /// Obtiene o establece el tamaño de la página. Si el valor establecido es mayor que MaxPageSize, se usa MaxPageSize.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

}
