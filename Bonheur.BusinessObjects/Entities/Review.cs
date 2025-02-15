using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class Review : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public string SummaryExperience { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        // Các tiêu chí đánh giá từ 1 đến 5
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


        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
