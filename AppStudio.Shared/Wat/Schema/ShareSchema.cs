using System.Runtime.Serialization;

namespace AppStudio.Wat.Schema
{
    [DataContract]
    public class ShareSchema
    {
        public ShareSchema()
        {
            IsEnabled = false;
        }
        [DataMember(Name = "enabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string ShareUrl { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

    }
}
