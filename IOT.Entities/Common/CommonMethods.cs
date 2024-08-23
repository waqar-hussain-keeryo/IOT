namespace IOT.Entities.Common
{
    public class CommonMethods
    {
        public static Guid ValidateGuid(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Guid.Empty;
            }

            return userGuid;
        }
    }
}
