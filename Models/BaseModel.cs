using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseModel
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }
    }

    public class BaseGuidModel
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }
    }
}