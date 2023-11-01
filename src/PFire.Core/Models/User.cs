namespace PFire.Core.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public int GameID { get; set; }
        public int GameIP { get; set; }
        public int GamePort { get; set; }
    }
}
