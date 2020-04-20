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
        //传输的过程中会用到密码，但是这个密码不应该被存入数据库中。
        [NotMapped]
        [DataMember]
        public string? Password { get; set; }
        //传输的过程中不会用到密码哈希值，但是哈希值需要存入数据库中。
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
