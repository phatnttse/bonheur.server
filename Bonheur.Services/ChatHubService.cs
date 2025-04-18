﻿using Bonheur.BusinessObjects.Entities;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Message;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Bonheur.BusinessObjects.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Bonheur.DAOs;
using System.Collections.Concurrent;

namespace Bonheur.Services
{
    [Authorize]
    public sealed class ChatHubService : Hub
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IRequestPricingsRepository _requestPricingsRepository;
        private readonly IMessageAttachmentRepository _messageAttachmentRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ChatHubService(
            IUserAccountRepository userAccountRepository,
            UserManager<ApplicationUser> userManager, 
            ISupplierRepository supplierRepository,
            IMessageRepository messageRepository, 
            IRequestPricingsRepository requestPricingsRepository,
            IMessageAttachmentRepository messageAttachmentRepository,
            IMapper mapper,
            ApplicationDbContext context
        )
        {
            _userAccountRepository = userAccountRepository;
            _userManager = userManager;
            _supplierRepository = supplierRepository;
            _messageRepository = messageRepository;
            _requestPricingsRepository = requestPricingsRepository;
            _messageAttachmentRepository = messageAttachmentRepository;
            _mapper = mapper;
            _context = context;
        }

        public static readonly ConcurrentDictionary<string, OnlineUserDTO> onlineUsers = new ();

        public override async Task OnConnectedAsync()
        {        
            try
            {
                var httpContext = Context.GetHttpContext();
                //var userId = httpContext?.Request.Query["uid"].ToString();
                var userId = Utilities.GetCurrentUserId();
                //var userName = Context!.User!.Identity!.Name;
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

                    //await Clients.AllExcept(connectionId).SendAsync("UserConnected", user);
                }

                await Clients.Caller.SendAsync("OnlineUsers", this.GetAllSuppliersContacted());
                
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
                var httpContext = Context.GetHttpContext();
                var userId = Utilities.GetCurrentUserId();
                var currentUser = await _userAccountRepository.GetUserByIdAsync(userId);

                List<Message> messages = await _messageRepository.GetMessages(userId, recipientId, pageNumber, pageSize);

                foreach (var message in messages)
                {
                    var msg = await _messageRepository.GetMessageByIdAsync(message.Id);

                    if (msg != null && msg.ReceiverId == userId)
                    {
                        msg.IsRead = true;
                        await _messageRepository.UpdateMessage(msg);
                    }
                }
                await Clients.Caller.SendAsync("ReceiveMessageList", _mapper.Map<List<MessageDTO>>(messages));

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

                Message newMessage = new Message
                {
                    SenderId = senderId,
                    SenderName = senderSupplier != null ? senderSupplier.Name : senderUser!.FullName,
                    SenderRole = senderSupplier != null ? Constants.Roles.SUPPLIER : Constants.Roles.USER,
                    ReceiverId = receiverId,
                    ReceiverName = receiverSupplier != null ? receiverSupplier.Name : receiverUser!.FullName,
                    ReceiverRole = receiverSupplier != null ? Constants.Roles.SUPPLIER : Constants.Roles.USER,
                    Content = message.Content,
                    IsRead = false
                };

                await _messageRepository.AddMessage(newMessage);

                if (message.AzureBlobUploadedResponses != null && message.AzureBlobUploadedResponses.Count > 0)
                {
                    foreach (var uploadedResponse in message.AzureBlobUploadedResponses)
                    {
                        var newMessageAttachment = new MessageAttachment
                        {
                            FileName = uploadedResponse.Blob.Name,
                            FilePath = uploadedResponse.Blob.Uri,
                            FileType = uploadedResponse.Blob.ContentType,
                            MessageId = newMessage.Id
                        };

                        await _messageAttachmentRepository.CreateMessageAttachmentAsync(newMessageAttachment);
                    }                   
                }

                if (message.RequestPricingId != null && int.IsPositive((int)message.RequestPricingId) && message.isSupplierReply != null && (bool)message.isSupplierReply)
                {
                    var requestPricing = await _requestPricingsRepository.GetRequestPricingById((int)message.RequestPricingId);

                    if (requestPricing != null && requestPricing.Status == RequestPricingStatus.Pending)
                    {
                        requestPricing.Status = RequestPricingStatus.Responded;
                        await _requestPricingsRepository.UpdateRequestPricingStatus(requestPricing);
                    }
                }

                var connectionId = onlineUsers.Values.FirstOrDefault(u => u.Id == receiverId)?.ConnectionId;

                if (connectionId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNewMessage", _mapper.Map<MessageDTO>(newMessage));
                    await Clients.Client(connectionId).SendAsync("ReceiveMessageNotification", senderSupplier != null ? senderSupplier.Name : senderUser!.FullName);
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

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Utilities.GetCurrentUserId();
            onlineUsers.Remove(userId!, out _);
            return Task.CompletedTask;
        }

        //private async Task<IEnumerable<OnlineUserDTO>> GetAllUsers()
        //{
        //    try
        //    {
        //        var userId = Utilities.GetCurrentUserId();

        //        var onlineUsersSet = new HashSet<string>(onlineUsers.Keys);

        //        var users = _userManager.Users.Select(u => new OnlineUserDTO
        //        {
        //            Id = u.Id,
        //            UserName = u.UserName,
        //            FullName = u.FullName,
        //            PictureUrl = u.PictureUrl,
        //            IsOnline = onlineUsersSet.Contains(u.Id),
        //            UnreadMessages = _context.Messages.Count(x => x.ReceiverId == userId && x.SenderId == u.Id && !x.IsRead)
        //        }).OrderByDescending(u => u.IsOnline).ToList();

        //        return await Task.FromResult(users);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Error in GetAllUsers: {ex.Message}");
        //        throw;
        //    }
        //}

        private Task<List<OnlineUserDTO>> GetAllSuppliersContacted()
        {
            try
            {
                var userId = Utilities.GetCurrentUserId();

                // Lấy danh sách ID của các supplier mà userId đã từng gửi tin nhắn
                var supplierIds = _messageRepository.GetSupplierIdsContacted(userId).Result;

                var onlineUsersSet = new HashSet<string>(onlineUsers.Keys);

                // Lấy số lượng tin nhắn chưa đọc theo từng supplier
                var unreadMessagesDict = _messageRepository.GetUnredMessagesBySupplierIds(userId, supplierIds).Result;

                var latestMessagesDict = _messageRepository.GetLatestMessageBySupplierIds(userId, supplierIds).Result;


                // Truy vấn danh sách suppliers từ UserManager một lần duy nhất
                var suppliers = (from u in _userManager.Users
                                 join s in _context.Suppliers on u.Id equals s.UserId
                                 where supplierIds.Contains(u.Id)
                                 select new OnlineUserDTO
                                 {
                                     Id = u.Id,
                                     UserName = u.UserName,
                                     FullName = s != null ? s.Name : u.FullName,
                                     PictureUrl = s != null ? s.Images!.Where(s => s.IsPrimary).FirstOrDefault()!.ImageUrl : u.PictureUrl,
                                     IsOnline = onlineUsersSet.Contains(u.Id),
                                     UnreadMessages = unreadMessagesDict.ContainsKey(u.Id) ? unreadMessagesDict[u.Id] : 0,
                                     LatestMessage = latestMessagesDict.ContainsKey(u.Id) ? latestMessagesDict[u.Id].LatestMessage : null,
                                     LatestMessageAt = latestMessagesDict.ContainsKey(u.Id) ? latestMessagesDict[u.Id].LatestMessageAt : null
                                 })
                                 .AsEnumerable()
                                 .OrderByDescending(u => u.LatestMessageAt)
                                 .ToList();


                return Task.FromResult(suppliers);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetAllSuppliersContacted: {ex.Message}");
                throw;
            }
        }

    }
}
