using System;

namespace PFire.Core.Entities
{
    public class Screenshot
    {
        public int Id { get; set; } = 0;
        public bool AdultContent { get; set; } = false;
        public string FileName { get; set; }
        public string Description { get; set; } = "";
        public DateTime Uploaded { get; set; }
        public DateTime Created { get; set; }
        public string GameShortName { get; set; } = "";
        public string GameLongName { get; set; } = "";
        public string Nickname { get; set; } = "";
        public int ViewCount { get; set; } = 0;
        public int ServerIp { get; set; } = 0;
        public int ServerPort { get; set; } = 0;
        public int GameId { get; set; } = 0;
        public int FileSize { get; set; } = 0;
        public int CreatedUnixTimestamp { get; set; }
        public int LockedState { get; set; } = 0;
    }
}
