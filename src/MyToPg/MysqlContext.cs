using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyToPg
{
    [Table("phones")]
    public class Phone
    {
        [Column("id")]
        public uint Id { get; set; }
        [Column("number")]
        public ulong Number { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        public List<PhoneInfo> PhoneInfos { get; set; } = new List<PhoneInfo>();
    }

    [Table("phone_infos")]
    public class PhoneInfo
    {
        [Column("id")]
        public uint Id { get; set; }
        [Column("phone_id")]
        public uint PhoneId { get; set; }
        public Phone Phone { get; set; }
        [Column("source_id")]
        public string SourceId { get; set; }
        [Column("id_source")]
        public string IdOnSource { get; set; }
        [Column("data")]
        public string Data { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class MysqlContext : DbContext
    {
        public DbSet<Phone> Phones { get; set; }
        public DbSet<PhoneInfo> PhoneInfos { get; set; }
        public MysqlContext(DbContextOptions options) : base(options)
        {

        }
    }
}
