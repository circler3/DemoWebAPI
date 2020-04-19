using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SurveyWebAPI.Models
{
    [DataContract]
    [Table("userinfo")]
    public class UserInfo 
    {
        [DataMember]
        [Key]
        public string UserName { get; set; } = default!;
        [NotMapped]
        [DataMember]
        public string? Password { get; set; }
        [IgnoreDataMember]
        public string? PasswordHash { get; set; }
        [DataMember]
        public string? Role { get; set; }

        public static string GetRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "User";
            return role.ToLower() switch
            {
                "admin" => "Admin",
                "supervisor" => "Supervisor",
                _ => "User"
            };
        }

    }
}
