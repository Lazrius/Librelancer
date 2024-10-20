using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale;

public struct AlchemyKeyFrameColor
{
    [XmlAttribute("keyframe")]
    public float Keyframe;

    [XmlElement("Color")]
    public Color3f Color;
}

public struct AlchemyKeyFrameValue
{
    [XmlAttribute("keyframe")]
    public float Keyframe;

    [XmlAttribute("value")]
    public float Value;
}
