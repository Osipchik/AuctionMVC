using Newtonsoft.Json;

namespace WebApplication4.DTO.Comment
{
    public class CommentMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("lotId")]
        public string LotId { get; set; }
    }
}