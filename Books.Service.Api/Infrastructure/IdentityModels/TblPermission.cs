using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Books.Service.Internal.Api.Infrastructure.IdentityModels
{
    public partial class TblPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string RoleId { get; set; } = null!;
        public string MenuId { get; set; } = null!;
    }
}

