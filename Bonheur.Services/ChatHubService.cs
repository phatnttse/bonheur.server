using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Message;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Cms;


namespace Bonheur.Services
{
    [Authorize]
    public sealed class ChatHubService : Hub
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ChatHubService(IUserAccountRepository userAccountRepository,
            UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userAccountRepository = userAccountRepository;
            _userManager = userManager;
            _context = dbContext;
        }

        public static readonly Dictionary<string, OnlineUserDTO> onlineUsers = new Dictionary<string, OnlineUserDTO>();

        public override async Task OnConnectedAsync()
        {        
            try
            {
                var httpContext = Context.GetHttpContext();
                var senderId = httpContext?.Request.Query["senderId"].ToString();
                var userId = Utilities.GetCurrentUserId();
                var userName = Context!.User!.Identity!.Name;
                var currentUser = await _userAccountRepository.GetUserByIdAsync(userId!);
                var connectionId = Context.ConnectionId;
                if (onlineUsers.ContainsKey(userId!))
                {
                    onlineUsers[userId!].ConnectionId = connectionId;
                }
                else
                {
                    var user = new OnlineUserDTO
                    {
                        ConnectionId = connectionId,
                        Id = currentUser?.Id,
                        UserName = currentUser?.UserName,
                        FullName = currentUser?.FullName,
                        PictureUrl = currentUser?.PictureUrl
                    };
                    onlineUsers.TryAdd(userId!, user);

                    await Clients.AllExcept(connectionId).SendAsync("UserConnected", user);
                }

                await Clients.All.SendAsync("OnlineUsers", this.GetAllUsers());
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task LoadMessages(string recipientId, int pageNumber = 1)
        {
            try
            {
                int pageSize = 10;
                var userId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found");
                var currentUser = await _userAccountRepository.GetUserByIdAsync(userId!);

                List<Message> messages = await _context.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == recipientId) ||
                                (m.SenderId == recipientId && m.ReceiverId == userId))
                    .OrderByDescending(m => m.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (var message in messages)
                {
                    var msg = await _context.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);

                    if (msg != null && msg.ReceiverId == userId)
                    {
                        msg.IsRead = true;
                        await _context.SaveChangesAsync();
                    }
                }

                await Clients.All.SendAsync("OnlineUsers", this.GetAllUsers());

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public async Task SendMessage(MessageRequestDTO message)
        {
            try
            {
                var senderId = Utilities.GetCurrentUserId() ?? throw new ApiException("User not found");
                var receiverId = message.ReceiverId ?? throw new Exception("Receiver not found");

                var newMessage = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = message.Content,
                    IsRead = false
                };

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                await Clients.User(receiverId).SendAsync("ReceiveNewMessage", newMessage);

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public async Task NotifyTyping(string recipientId)
        {
            var senderId = Utilities.GetCurrentUserId() ?? throw new Exception("User not found");

            var connectionId = onlineUsers.Values.FirstOrDefault(u => u.Id == recipientId)?.ConnectionId;

            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveTypingNotification", senderId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Utilities.GetCurrentUserId() ?? throw new Exception("User not found");
            onlineUsers.Remove(userId!, out _);
            await Clients.All.SendAsync("OnlineUsers", this.GetAllUsers());
        }

        private async Task<IEnumerable<OnlineUserDTO>> GetAllUsers()
        {
            var currentUserId = Utilities.GetCurrentUserId() ?? throw new Exception("User not found");

            var onlineUsersSet = new HashSet<string>(onlineUsers.Keys);

            var users = await _userManager.Users.Select(u => new OnlineUserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                PictureUrl = u.PictureUrl,
                IsOnline = onlineUsersSet.Contains(u.Id),
                UnreadMessages = _context.Messages
                    .Where(m => m.ReceiverId == currentUserId && m.SenderId == u.Id && !m.IsRead)
                    .Count()
            }).OrderByDescending(u => u.IsOnline).ToListAsync();

            return users;
        }
    }
}
