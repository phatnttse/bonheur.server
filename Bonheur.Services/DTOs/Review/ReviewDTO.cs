using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Review
{
    public class ReviewDTO
    {
        public string UserId { get; set; }

        public int SupplierId { get; set; }

        public string SummaryExperience { get; set; }

        public string Content { get; set; }

        [Required]
        [Range(1, 5)]
        // Chất lượng dịch vụ
        public int QualityOfService { get; set; }

        [Required]
        [Range(1, 5)]
        //Thời gian phản hồi
        public int ResponseTime { get; set; }

        [Required]
        [Range(1, 5)]
        // Tính chuyên nghiệp
        public int Professionalism { get; set; }

        [Required]
        [Range(1, 5)]
        // Giá cả
        public int ValueForMoney { get; set; }

        [Required]
        [Range(1, 5)]
        // Tính linh hoạt
        public int Flexibility { get; set; }

        public virtual UserAccountDTO User { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }


    }
}
