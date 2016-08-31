using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OlxLib.Workers;

namespace OlxLib.Entities
{
    public class ExportJob 
    {
        public int Id { get; set; }
        public int DownloadJobId { get; set; }
        public DownloadJob DownloadJob { get; set; }

        [NotMapped]
        public OlxAdvert Data { get; set; }
        [Column("Data")]
        public string DataSerialized
        {
            get
            {
                return JsonConvert.SerializeObject(Data);
            }
            set
            {
                Data = JsonConvert.DeserializeObject<OlxAdvert>(value, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
        } 
        public DateTime CreateAt { get; set; }
    }
}
