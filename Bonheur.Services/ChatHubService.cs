using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Message;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace Bonheur.Services
{
    [Authorize]
    public sealed class ChatHubService : Hub
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ISupplierRepository _supplierRepository;

        public ChatHubService(IUserAccountRepository userAccountRepository,
            UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, ISupplierRepository supplierRepository)
        {
            _userAccountRepository = userAccountRepository;
            _userManager = userManager;
            _context = dbContext;
            _supplierRepository = supplierRepository;
        }

        public static readonly Dictionary<string, OnlineUserDTO> onlineUsers = new Dictionary<string, OnlineUserDTO>();

        public override async Task OnConnectedAsync()
        {        
            try
            {
                var httpContext = Context.GetHttpContext();
                //var userId = httpContext?.Request.Query["uid"].ToString();
                var userId = Utilities.GetCurrentUserId();
                //var userName = Context!.User!.Identity!.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
                int pageSize = 20;
                var httpContext = Context.GetHttpContext();
                var userId = Utilities.GetCurrentUserId();
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                List<Message> messages = await _context.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == recipientId) ||
                                (m.SenderId == recipientId && m.ReceiverId == userId))
                    .OrderBy(m => m.CreatedAt)
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
                await Clients.Caller.SendAsync("ReceiveMessageList", messages);

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
                var senderId = Utilities.GetCurrentUserId() ?? throw new Exception("Sender not found");
                var receiverId = message.ReceiverId ?? throw new Exception("Receiver not found");

                var senderSupplier = await _supplierRepository.GetSupplierByUserIdAsync(senderId);
                var senderUser = (ApplicationUser?)null;

                if (senderSupplier == null)
                {
                    senderUser = await _userAccountRepository.GetUserByIdAsync(senderId);

                    if (senderUser == null)
                    {
                        throw new Exception("Sender not found");
                    }
                }

                var receiverSupplier = await _supplierRepository.GetSupplierByUserIdAsync(receiverId);
                var receiverUser = (ApplicationUser?)null;

                if (receiverSupplier == null)
                {
                     receiverUser = await _userAccountRepository.GetUserByIdAsync(receiverId);

                    if (receiverUser == null)
                    {
                        throw new Exception("Receiver not found");
                    }
                }

                var newMessage = new Message
                {
                    SenderId = senderId,
                    SenderName = senderSupplier != null ? senderSupplier.Name : senderUser!.FullName,
                    ReceiverId = receiverId,
                    ReceiverName = receiverSupplier != null ? receiverSupplier.Name : receiverUser!.FullName,
                    Content = message.Content,
                    IsRead = false
                };

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                var connectionId = onlineUsers.Values.FirstOrDefault(u => u.Id == receiverId)?.ConnectionId;

                if (connectionId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNewMessage", newMessage);
                }
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
            var senderId = Utilities.GetCurrentUserId() ?? throw new Exception("Sender not found");

            var senderSupplier = await _supplierRepository.GetSupplierByUserIdAsync(senderId);
            var senderUser = (ApplicationUser?)null;

            if (senderSupplier == null)
            {
                senderUser = await _userAccountRepository.GetUserByIdAsync(senderId);

                if (senderUser == null)
                {
                    throw new Exception("Sender not found");
                }
            }

            var connectionId = onlineUsers.Values.FirstOrDefault(u => u.Id == recipientId)?.ConnectionId;

            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveTypingNotification", senderSupplier != null ? senderSupplier.Name : senderUser!.FullName);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = Utilities.GetCurrentUserId();
            onlineUsers.Remove(userId!, out _);
            await Clients.All.SendAsync("OnlineUsers", this.GetAllUsers());
        }

        private async Task<IEnumerable<OnlineUserDTO>> GetAllUsers()
        {
            try
            {
                var userId = Utilities.GetCurrentUserId();

                var onlineUsersSet = new HashSet<string>(onlineUsers.Keys);

                var users = _userManager.Users.Select(u => new OnlineUserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    PictureUrl = u.PictureUrl,
                    IsOnline = onlineUsersSet.Contains(u.Id),
                    UnreadMessages = _context.Messages.Count(x => x.ReceiverId == userId && x.SenderId == u.Id && !x.IsRead)
                }).OrderByDescending(u => u.IsOnline).ToList();

                return await Task.FromResult(users);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetAllUsers: {ex.Message}");
                throw;
            }
        }

    }
}
