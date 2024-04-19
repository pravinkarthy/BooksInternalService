using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Books.Service.Internal.Api.Infrastructure.IdentityModels
{
    public partial class TblRefreshtoken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? TokenId { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsActive { get; set; }
    }
}

