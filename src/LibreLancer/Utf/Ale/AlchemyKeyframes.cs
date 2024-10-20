using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale;

public struct AlchemyKeyFrameColor
{
    [XmlAttribute("time")]
    public float Time;

    [XmlElement("Color")]
    public Color3f Color;
}

public struct AlchemyKeyFrameValue
{
    [XmlAttribute("time")]
    public float Time;

    [XmlAttribute("value")]
    public float Value;
}
