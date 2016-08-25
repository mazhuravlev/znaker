using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using PostgreSqlProvider;

namespace MyToPg
{
    public class PostgresContext : PostgreSqlContext
    {
        public PostgresContext(DbContextOptions options) : base(options)
        {
        }
    }
}
