using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Enums
{
    public enum OrderStatus
    {
        /// <summary>
        /// Đơn hàng mới được tạo và đang chờ thanh toán.
        /// </summary>
        PendingPayment,

        /// <summary>
        /// Gói đăng ký đã được kích hoạt và đang trong thời gian hiệu lực.
        /// </summary>
        Active,

        /// <summary>
        /// Gói đăng ký đã hết hạn.
        /// </summary>
        Expired
    }
}
