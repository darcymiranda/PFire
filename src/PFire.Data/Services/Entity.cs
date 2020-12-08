using System;
using System.ComponentModel.DataAnnotations;

namespace PFire.Data.Services
{
    public abstract class Entity
    {
        [Timestamp]
        public byte[] Version { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }
    }
}
