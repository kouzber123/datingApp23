namespace API.Helpers
{
  public class UserParams
  {
    //we allow client to use but set max param
    //custom parameters to get list of users and custom
    //USER PARAMS works as our app filter

    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; // set limiter
    }

    public string CurrentUsername { get; set; }
    public string Gender { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;

    public string OrderBy { get; set; } = "lastActive";

  }
}
