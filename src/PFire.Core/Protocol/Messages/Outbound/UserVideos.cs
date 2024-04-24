using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol;
using System.Collections.Generic;

/* 
 * Packet 179 - User Videos
 * This is a list pulled from the server that are remote videos by you.
 * 
 */

internal sealed class UserVideos : XFireMessage
{
    public UserVideos() : base(XFireMessageType.UserVideos)
    {
        //TODO: Have a Database of user uploaded videos, fetched by userid, populating the data from the database.
    }

    [XMessageField(0x93)]
    public List<int> VideoId { get; set; } = new();
    [XMessageField(0x94)]
    public List<string> Title { get; set; } = new();
    [XMessageField(0x55)]
    public List<int> FileSize { get; set; } = new();
    [XMessageField(0x95)]
    public List<int> Width { get; set; } = new();
    [XMessageField(0x96)]
    public List<int> Height { get; set; } = new();
    [XMessageField(0x97)]
    public List<int> Length { get; set; } = new();
    // 0x98 seems to divide the length, by the value given. Not sure why.
    [XMessageField(0x98)]
    public List<int> TimeDivision { get; set; } = new();
    [XMessageField(0x21)]
    public List<int> GameId { get; set; } = new();
    [XMessageField(0x50)]
    public List<int> Created { get; set; } = new();
    [XMessageField(0x54)]
    public List<string> Description { get; set; } = new();
}
