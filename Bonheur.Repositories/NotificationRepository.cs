using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Bonheur.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO _notificationDAO;

        public NotificationRepository(NotificationDAO notificationDAO)
        {
            _notificationDAO = notificationDAO;
        }

        public async Task CreateNotificationAsync(Notification notification) => await _notificationDAO.CreateNotificationAsync(notification);

        public async Task DeleteNotificationAsync(Notification notification) => await _notificationDAO.DeleteNotificationAsync(notification);

        public async Task<Notification?> GetNotificationAsync(int id) => await _notificationDAO.GetNotificationAsync(id);

        public async Task<IPagedList<Notification>> GetNotificationsAsync(string userId, int pageNumber, int pageSize) => await _notificationDAO.GetNotificationsAsync(userId, pageNumber, pageSize);

        public async Task UpdateNotificationAsync(Notification notification) => await _notificationDAO.UpdateNotificationAsync(notification);
        public async Task AddRangeAsync(List<Notification> notifications) => await _notificationDAO.AddRangeAsync(notifications);

    }
}
