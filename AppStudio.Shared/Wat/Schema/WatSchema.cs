using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace AppStudio.Wat.Schema
{
        [DataContract]
        public class WatSchema
        {
            public WatSchema() { }

            public WatSchema(string jsonConfigFile)
            {
            }
            
            public Uri BaseURL { get { return new Uri(BaseURLString); } }

            [DataMember(Name = "baseURL")]
            public string BaseURLString { get; set; }

            public Uri MobileBaseURL { get { return new Uri(MobileURLString); } }

            [DataMember(Name = "mobileURL")]
            public string MobileURLString { get; set; }

            [DataMember(Name = "share")]
            public ShareSchema Share { get; set; }

            [DataMember(Name = "appBar")]
            public AppBarSchema AppBar { get; set; }

            [DataMember(Name = "navBar")]
            public NavBarSchema NavBar { get; set; }          

            [DataMember(Name = "styleTheme")]
            public string StyleTheme { get; set; }

            public static WatSchema CreateConfig(string sourceJSON)
            {
                return JsonConvert.DeserializeObject<WatSchema>(sourceJSON);
            }

            public static string CreateJson(WatSchema config)
            {
                return JsonConvert.SerializeObject(config);
            }
            private bool getJSONBool(string boolVal)
            {
                return (boolVal == "true");
            }

        }
    

}
