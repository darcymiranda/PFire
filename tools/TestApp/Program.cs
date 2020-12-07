using PFire.Client;

using System.Threading;


namespace TestApp
{
    class Program
    {
        
        /*
        static XFireClient Login()
        {
            

            
            LoginChallenge challenge = new LoginChallenge();
            xfireClient.SendAndProcessMessage(challenge);

            LoginRequest login = new LoginRequest();
            xfireClient.SendAndProcessMessage(login);
            
            return xfireClient;
        }
        */

        static void Main(string[] args)
        {

            XFireUser user1 = new XFireUser("localhost", 25999, "b", "71fa140c7fa2fff09ff4de0a1a273510b93013c6");
            user1.Connect();

            //Console.WriteLine("Waiting...");

            user1.SendFriendRequest("c", "Add me");
            user1.SendFriendRequest("a", "Add me");

            while (true)
            {
                Thread.Sleep(3000);
            }


            /*
            using (TcpClient client = new TcpClient())
            {
                client.Connect("localhost", 25999);
                XFireClient xfireClient = new XFireClient(client);

                Func<string, int, XFireClient> LoginUser = (hostName, port) =>
                {
                    TcpClient client = new TcpClient();
                    client.Connect(hostName, port);
                    return new XFireClient(client); 
                };

                Func<string, string, string> HashPassword = (username, pwd) =>
                {
                    string hashedPwd = username + pwd + "UltimateArena";
                    byte[] hash = SHA1.HashData(Encoding.ASCII.GetBytes(hashedPwd));
                    string pwd_2 = $"0x{hashedPwd:X}" + "4dc383ea21bf4bca83ea5040cb10da62";
                    byte[] hash_2 = SHA1.HashData(Encoding.ASCII.GetBytes(pwd_2));
                    return Encoding.ASCII.GetString(hash_2);
                };

                xfireClient.User = new PFire.Database.User();
                xfireClient.User.Username = "b";
                xfireClient.User.Password = HashPassword(xfireClient.User.Username, xfireClient.User.Password);

                xfireClient.InitializeClient();
                LoginChallenge challenge = new LoginChallenge();
                xfireClient.SendMessage(challenge);

                LoginRequest login = new LoginRequest();
                xfireClient.SendMessage(login);
                

                //XFireMessage. msg = new XFireMessage(XFireMessageType.FriendRequest);

                return 0;
            */
        }
    }
}
