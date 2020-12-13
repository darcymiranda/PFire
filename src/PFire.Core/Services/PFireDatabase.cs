using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PFire.Core.Models;
using PFire.Data.Entities;
using PFire.Data.Services;

namespace PFire.Core.Services
{
    internal interface IPFireDatabase
    {
        Task<UserModel> InsertUser(string username, string password, string salt);
        Task InsertMutualFriend(UserModel me, UserModel them);
        Task InsertFriendRequest(UserModel me, UserModel them, string message);
        Task<UserModel> QueryUser(string username);
        Task<List<UserModel>> QueryUsers(string username);
        Task<List<UserModel>> QueryFriends(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel otherUser);
        Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] thems);
        Task UpdateNickname(UserModel user, string nickname);
    }

    internal class PFireDatabase : IPFireDatabase
    {
        private readonly IServiceProvider _serviceProvider;

        public PFireDatabase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<UserModel> InsertUser(string username, string password, string salt)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandTransactionProvider = scope.ServiceProvider.GetRequiredService<ICommandTransactionProvider>();
            using (var transaction = await commandTransactionProvider.StartTransaction())
            {
                //assume valid for now
                await transaction.CreateEntity<User>().Run(x => x.Username = username).Run(x => x.Password = password).Run(x => x.Salt = salt).SaveChanges();

                //assume valid for now
                await transaction.Commit();
            }

            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(x => x.Username == username)
                               .Select(x => new UserModel
                               {
                                   Id = x.Id,
                                   Username = x.Username,
                                   Password = x.Password,
                                   Nickname = x.Nickname
                               })
                               .SingleOrDefaultAsync();
        }

        public async Task InsertMutualFriend(UserModel me, UserModel them)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandTransactionProvider = scope.ServiceProvider.GetRequiredService<ICommandTransactionProvider>();

            using (var transaction = await commandTransactionProvider.StartTransaction())
            {
                //assume valid for now
                await transaction.DeleteEntity<Friend>(me.Id, them.Id);
                await transaction.DeleteEntity<Friend>(them.Id, me.Id);

                await transaction.CreateEntity<Friend>().Run(x => x.MeId = me.Id).Run(x => x.ThemId = them.Id).SaveChanges();
                await transaction.CreateEntity<Friend>().Run(x => x.ThemId = me.Id).Run(x => x.MeId = them.Id).SaveChanges();

                //assume valid for now
                await transaction.Commit();
            }
        }

        public async Task InsertFriendRequest(UserModel me, UserModel them, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandTransactionProvider = scope.ServiceProvider.GetRequiredService<ICommandTransactionProvider>();

            using (var transaction = await commandTransactionProvider.StartTransaction())
            {
                //assume valid for now
                await transaction.CreateEntity<Friend>()
                                 .Run(x => x.MeId = me.Id)
                                 .Run(x => x.ThemId = them.Id)
                                 .Run(x => x.Pending = true)
                                 .Run(x => x.Message = message)
                                 .SaveChanges();

                //assume valid for now
                await transaction.Commit();
            }
        }

        public async Task<UserModel> QueryUser(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(a => a.Username == username)
                               .Select(x => new UserModel
                               {
                                   Id = x.Id,
                                   Username = x.Username,
                                   Password = x.Password,
                                   Nickname = x.Nickname
                               })
                               .FirstOrDefaultAsync();
        }

        public async Task<List<UserModel>> QueryUsers(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(a => a.Username.Contains(username))
                               .Select(x => new UserModel
                               {
                                   Id = x.Id,
                                   Username = x.Username,
                                   Nickname = x.Nickname
                               })
                               .ToListAsync();
        }

        public async Task<List<UserModel>> QueryFriends(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(a => a.Id == user.Id)
                               .SelectMany(a => a.MyFriends)
                               .Where(x => x.Pending == false)
                               .Select(x => new UserModel
                               {
                                   Id = x.Them.Id,
                                   Username = x.Them.Username,
                                   Nickname = x.Them.Nickname
                               })
                               .ToListAsync();
        }

        public async Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(a => a.Id == user.Id)
                               .SelectMany(x => x.MyFriends)
                               .Where(x => x.Pending)
                               .Select(x => new FriendRequestModel
                               {
                                   Id = x.Them.Id,
                                   Username = x.Them.Username,
                                   Nickname = x.Them.Nickname,
                                   Message = x.Message
                               })
                               .ToListAsync();
        }

        public async Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(a => a.Id == user.Id)
                               .SelectMany(x => x.FriendsOf)
                               .Where(x => x.Pending)
                               .Select(x => new FriendRequestModel
                               {
                                   Id = x.Them.Id,
                                   Username = x.Them.Username,
                                   Nickname = x.Them.Nickname,
                                   Message = x.Message
                               })
                               .ToListAsync();
        }

        public async Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] thems)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandTransactionProvider = scope.ServiceProvider.GetRequiredService<ICommandTransactionProvider>();

            using (var transaction = await commandTransactionProvider.StartTransaction())
            {
                foreach (var them in thems)
                {
                    //assume valid for now
                    await transaction.DeleteEntity<Friend>(me.Id, them.Id);
                    await transaction.DeleteEntity<Friend>(them.Id, me.Id);
                }

                //assume valid for now
                await transaction.Commit();
            }
        }

        public async Task UpdateNickname(UserModel user, string nickname)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandTransactionProvider = scope.ServiceProvider.GetRequiredService<ICommandTransactionProvider>();

            using (var transaction = await commandTransactionProvider.StartTransaction())
            {
                //assume valid for now
                await transaction.UpdateEntity<User>(user.Id).Run(x => x.Nickname = nickname).SaveChanges();

                //assume valid for now
                await transaction.Commit();
            }
        }
    }
}
