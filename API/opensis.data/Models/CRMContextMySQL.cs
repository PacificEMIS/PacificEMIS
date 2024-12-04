using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace opensis.data.Models
{
    public class CRMContextMySQL : CRMContext
    {
        private readonly DbContextOptions? contextOptions;
        public CRMContextMySQL() { }
        public CRMContextMySQL(DbContextOptions options) : base(options)
        {
            this.contextOptions = options;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //var tenant = "fedsis_new";
                //** ML Server **//
                string connectionString = "server=110.227.203.159;port=3307;database=fedsis_new;user=admin;password=methodolog1c;default command timeout=3000";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }

        }
    }
}
