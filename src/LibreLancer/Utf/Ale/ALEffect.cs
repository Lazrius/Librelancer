// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale;

public struct AleEffectPair
{
    [XmlAttribute("source")] public uint Source;
    [XmlAttribute("target")] public uint Target;
}

public class ALEffect
{
    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("crc")]
    public uint CRC;

    [XmlArray("tree")]
    [XmlArrayItem("ref")]
    public List<AlchemyNodeRef> FxTree;

    [XmlArray("fx")]
    [XmlArrayItem("ref")]
    public List<AlchemyNodeRef> Fx;

    [XmlArray("pairs")]
    [XmlArrayItem("pair")]
    public List<AleEffectPair> Pairs;

    public ALEffect ()
    {
    }

    public AlchemyNodeRef FindRef(uint index)
    {
        var result = from AlchemyNodeRef r in Fx where r.Index == index select r;
        if (result.Count() == 1)
            return result.First();
        throw new Exception();
    }
}
