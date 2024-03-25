namespace PFire.Core.Models
{
    public class UserModel
    {
        public uint Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public GameModel Game { get; set; } = new GameModel();
    }
}
