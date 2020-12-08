using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PFire.Data.Services;

namespace PFire.Data.Entities
{
    internal class User : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Salt { get; set; }

        public string Nickname { get; set; }

        public List<Friend> MyFriends { get; set; }
        public List<Friend> FriendsOf { get; set; }
    }
}
