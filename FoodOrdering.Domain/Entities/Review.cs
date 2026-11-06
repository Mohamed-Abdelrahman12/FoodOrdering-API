

using FoodOrdering.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace FoodOrdering.Domain.Entities
{
 
    
        public class Review : BaseEntity
        {
            public string UserId { get; set; } = string.Empty;
            public Guid ProductId { get; set; }
            public Guid? OrderId { get; set; }
             [Range(1 ,5)]
            public int Rating { get; set; }
            public string? Comment { get; set; }
            public bool IsApproved { get; set; } = false;
            public ApplicationUser User { get; set; } = null!;
            public Product Product { get; set; } = null!;
            //public Order? Order { get; set; }
        }
    }

