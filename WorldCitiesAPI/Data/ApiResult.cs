

namespace WorldCitiesAPI.Data
{
  public class ApiResult<T>
  {
    #region Properties
    public List<T> Data { get; private set; }

    public int PageIndex { get; private set; }

    public int PageSize { get; private set; }

    public int TotalCount { get; private set; }

    public int TotalPages { get; private set; }

    public bool HasPreviousPage
    {
      get
      {
        return (PageIndex > 0);
      }
    }

    public bool HasNextPage
    {
      get
      {
        return ( (PageIndex + 1) < TotalPages  );
      }
    }

    #endregion


    private ApiResult(List<T> data, int count, int pageIndex, int pageSize)
    { 
    
    }
  }
}
