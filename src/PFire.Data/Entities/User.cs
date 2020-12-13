using System.Collections.Generic;

namespace PFire.Data.Entities
{
    public class User : Entity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Nickname { get; set; }
        public List<Friend> MyFriends { get; set; }
        public List<Friend> FriendsOf { get; set; }
    }
}
