namespace API.Entities
{
    public class UserLike
    {/// <summary>
    /// join table for likes
    /// </summary>
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        public AppUser TargetUser { get; set; }
        public int TargetUserId { get; set; }
    }
}
