using System.Xml.Serialization;
using LibreLancer.Utf.Mat;

namespace LibreLancer.ContentEdit;

[XmlRoot("Voice")]
public class UtfVoice
{
    public class VoiceLine
    {
        [XmlAttribute("id")]
        public uint Id;

        [XmlAttribute("data")]
        public byte[] Data;
    }

    [XmlElement("Line")]
    public VoiceLine[] Lines;
}
