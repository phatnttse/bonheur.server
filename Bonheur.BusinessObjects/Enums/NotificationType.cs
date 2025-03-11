using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Enums
{
    public enum NotificationType
    {
        Broadcast,     // Thông báo phát sóng
        Promotion,     // Khuyến mãi
        SystemAlert,   // Cảnh báo hệ thống
        General,       // Thông báo chung
        Payment,       // Thông báo thanh toán
        Order,         // Thông báo đơn hàng
        Account,       // Thông báo tài khoản
        Other          // Thông báo khác
    }
}
