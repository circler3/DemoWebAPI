using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyWebAPI.DataContext
{
    public class ManageDataContext : DbContext
    {
        public DbSet<UserInfo>  UserInfos { get; set; } = default!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(ConfigurationManager.Configuration.GetConnectionString("ServerConnection"));//配置连接字符串
        }
    }

}

