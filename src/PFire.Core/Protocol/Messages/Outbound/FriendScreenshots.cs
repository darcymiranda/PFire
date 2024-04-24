using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol;
using PFire.Core.Session;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Entities;

/* 
 * Packet 157 - Remote User Screenshots
 * This is a list pulled from the server that are remote screenshots by the user.
 * Interestingly, if you push the packet every log in, it will just add in the client itself.
 * 
 */

internal sealed class FriendScreenshots : XFireMessage
{
    public FriendScreenshots(int userid, IEnumerable<Screenshot> screenshots) : base(XFireMessageType.FriendScreenshots)
    {
        foreach (var screen in screenshots)
        {
            UserId = userid;
            ScreenshotIds.Add(screen.Id);
            GameIds.Add(screen.GameId);
            ServerIps.Add(screen.ServerIp);
            GamePorts.Add(screen.ServerPort);
            Descriptions.Add(screen.Description);
            LockedState.Add(screen.LockedState);
            Filenames.Add(screen.FileName);
        }
    }
    [XMessageField(0x01)]
    public int UserId { get; set; } = new();
    [XMessageField(0x5c)]
    public List<int> ScreenshotIds { get; set; } = new();
    [XMessageField(0x21)]
    public List<int> GameIds { get; set; } = new();
    [XMessageField(0x22)]
    public List<int> ServerIps { get; set; } = new();
    [XMessageField(0x23)]
    public List<int> GamePorts { get; set; } = new();
    [XMessageField(0x54)]
    public List<string> Descriptions { get; set; } = new();
    [XMessageField(0x50)]
    public List<int> LockedState { get; set; } = new();
    [XMessageField(0x5e)]
    public List<string> Filenames { get; set; } = new();
}
