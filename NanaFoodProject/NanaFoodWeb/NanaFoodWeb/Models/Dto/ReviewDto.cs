﻿using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class ReviewDto
    {
        public Guid ReviewId { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Hãy cho biết đánh giá của bạn về món ăn")]
        public string Comment { get; set; }
        [Range(1, 5)]
        public double Rating { get; set; }
        [Required(ErrorMessage = "Mã người dùng không thể trống")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Mã sản phẩm không thể trống")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Mã đơn hàng không thể trống")]
        public int OrderId { get; set; }
        public bool IsConfirm { get; set; }
        public DateTime ReviewedDate { get; set; } = DateTime.Now;
    }
}
