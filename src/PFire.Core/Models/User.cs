namespace PFire.Core.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public GameModel Game { get; set; } = new GameModel();
        public ClientPreferencesModel ClientPreferences { get; set; } = new ClientPreferencesModel();
    }
}
