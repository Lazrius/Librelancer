using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale;

[XmlInclude(typeof(AlchemyBool))]
[XmlInclude(typeof(AlchemyColorAnimation))]
[XmlInclude(typeof(AlchemyCurveAnimation))]
[XmlInclude(typeof(AlchemyFloat))]
[XmlInclude(typeof(AlchemyFloatAnimation))]
[XmlInclude(typeof(AlchemyInteger))]
[XmlInclude(typeof(AlchemyBlendInfo))]
[XmlInclude(typeof(AlchemyString))]
[XmlInclude(typeof(AlchemyTransform))]
public class AlchemyValue
{
}

public class AlchemyBool : AlchemyValue
{
    [XmlText] public bool Value;
}

public class AlchemyInteger : AlchemyValue
{
    [XmlText] public int Value;
}

public class AlchemyFloat : AlchemyValue
{
    [XmlText] public float Value;
}

public class AlchemyString : AlchemyValue
{
    [XmlText] public string Value;
}

public enum BlendFactor : byte
{
    Invalid = 0,
    Zero,
    One,
    SrcColor,
    InvSrcColor,
    SrcAlpha,
    DestAlpha,
    InvDestAlpha,
    DestColor,
    InvDestColor,
    SrcAlphaSat
}

public class AlchemyBlendInfo : AlchemyValue
{
    [XmlAttribute("srcFactor")] public BlendFactor SrcFactor;
    [XmlAttribute("destFactor")] public BlendFactor DstFactor;
}
