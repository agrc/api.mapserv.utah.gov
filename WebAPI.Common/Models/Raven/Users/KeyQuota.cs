namespace WebAPI.Common.Models.Raven.Users
{
    public class KeyQuota
    {
        public KeyQuota(int keysAllowed)
        {
            KeysAllowed = keysAllowed;
            KeysUsed = 0;
        }

        public int KeysUsed { get; set; }
        public int KeysAllowed { get; set; }
    }
}