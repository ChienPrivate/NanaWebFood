﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    [Table("Order")]
    public class Order
    {
        public int OrderId { get; set; }
        [Required(ErrorMessage = "")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "")]
        public string Address { get; set; }
        [Required(ErrorMessage = "")]
        public string PaymentType { get; set; }
        [Required(ErrorMessage = "")]
        public string PaymentStatus { get; set; }
        [Required(ErrorMessage = "")]
        public string OrderStatus { get; set; }
        [Required(ErrorMessage = "")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "")]
        public DateTime ReceiveDate { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
