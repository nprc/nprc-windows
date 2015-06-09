using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppStudio.Wat.Schema
{
    [DataContract]
    public class NavBarSchema : AppBarSchema
    {
        public NavBarSchema()
        {         
            Buttons = new List<BarButton>();
        }
    }

    [DataContract]
    public class BarButton
    {
        public BarButton() { }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "iconUrl")]
        public string iconUrl { get; set; }

        public Uri IconUri { get { return new Uri(iconUrl, UriKind.Relative); } }

        public Uri Action { get { return new Uri(ActionString); } }

        [DataMember(Name = "action")]
        public string ActionString { get; set; }

    }
}
