using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GGMTG.Server.Models;
//using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;


[Index(nameof(Username), nameof(Email), IsUnique = true)]
public record Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustId { get; set; }
    [MaxLength(50)]
    public required string Username { get; set; }
    [MaxLength(512)]
    public required byte[] Password { get; set; }
    [MaxLength(125)]
    public required string Email { get; set; }
    [Column(TypeName = "DateTime2")]
    public required DateTime Joined { get; init; }

    //public ICollection<Project>? Projects { get; set; }
}
