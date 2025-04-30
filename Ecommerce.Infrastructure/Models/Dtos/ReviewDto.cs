using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        [Range(1, 5)]
        public int Stars { get; set; }

        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }

        // Additional properties for display
        public string? ProductName { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        public Guid UserId { get; set; }
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }

        public string? Comment { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
    }

    public class UpdateReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }

        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
    }
}
