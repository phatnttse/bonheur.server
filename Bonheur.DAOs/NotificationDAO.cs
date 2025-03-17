using Bonheur.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Bonheur.DAOs
{
    public class NotificationDAO
    {
        private readonly ApplicationDbContext _context;

        public NotificationDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<Notification>> GetNotificationsAsync(string userId, int pageNumber, int pageSize)
        {
            var result = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return result.ToPagedList(pageNumber, pageSize);

        }

        public async Task<Notification?> GetNotificationAsync(int id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Notification> notifications)
        {
            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
