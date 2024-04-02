using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace ReviaRace
{
    public class HediffTexture
    {
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, nameof(hediff), xmlRoot.Name, null, null, null);
            if (xmlRoot.HasChildNodes)
            {
                this.path = xmlRoot.FirstChild.Value;
            }
        }
        public HediffDef hediff;
        public string path;
    }
}
