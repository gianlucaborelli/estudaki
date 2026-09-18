namespace Estudaki.Commons.Core.Models;

public abstract record PagedQuery
{
    public int Page { get; set; } = 0;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value > 25 || value < 1)
            {
                _pageSize = 10;
            }
            else
            {
                _pageSize = value;
            }
        }
    }
    private int _pageSize = 10;
    public string? SortLabel { get; set; }
    public string? SortDirection { get; set; }
}
