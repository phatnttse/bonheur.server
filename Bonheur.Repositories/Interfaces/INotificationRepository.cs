using Bonheur.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Bonheur.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IPagedList<Notification>> GetNotificationsAsync(string userId, int pageNumber, int pageSize);
        Task<Notification?> GetNotificationAsync(int id);
        Task CreateNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(Notification notification);
        Task AddRangeAsync(List<Notification> notifications);
    }
}
