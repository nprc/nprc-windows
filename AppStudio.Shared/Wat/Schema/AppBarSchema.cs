using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppStudio.Wat.Schema
{
    [DataContract]
    public class AppBarSchema
    {
        public AppBarSchema()
        {           
            Buttons = new List<BarButton>();
        }       

        [DataMember(Name = "buttons")]
        public List<BarButton> Buttons { get; set; }

        public void AddButton(string label, string icon, string action)
        {
            Buttons.Add(basicBtnSetup(label, icon, action));
        }

        public void AddButton(string label, string icon, string iconUri, string action)
        {
            BarButton nbb = basicBtnSetup(label, icon, action);
            Buttons.Add(nbb);
        }

        private BarButton basicBtnSetup(string label, string icon, string action)
        {
            BarButton nbb = new BarButton()
            {
                Label = label,
                Icon = icon,
                ActionString = action
            };
            return nbb;
        }
    }
}
