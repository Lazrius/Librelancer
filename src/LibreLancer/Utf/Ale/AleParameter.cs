// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Xml;
using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale
{
	public class AleParameter
	{
		[XmlAttribute("name")]
		public string Name;

        [XmlElement("Bool", typeof(AlchemyBool))]
        [XmlElement("String", typeof(AlchemyString))]
        [XmlElement("Integer", typeof(AlchemyInteger))]
        [XmlElement("Float", typeof(AlchemyFloat))]
        [XmlElement("CurveAnimation", typeof(AlchemyCurveAnimation))]
        [XmlElement("ColorAnimation", typeof(AlchemyColorAnimation))]
        [XmlElement("Pair", typeof(AlchemyPair))]
        [XmlElement("Transform", typeof(AlchemyTransform))]
        [XmlElement("FloatAnimation", typeof(AlchemyFloatAnimation))]
		public AlchemyValue Value;

		public override string ToString()
		{
			return $"[{Name}: {Value}]";
		}
	}
}

