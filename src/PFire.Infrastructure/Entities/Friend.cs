﻿namespace PFire.Infrastructure.Entities
{
    public class Friend : Entity
    {
        public int MeId { get; set; }
        public User Me { get; set; }

        public int ThemId { get; set; }
        public User Them { get; set; }

        public string Message { get; set; }

        public bool Pending { get; set; }
    }
}
