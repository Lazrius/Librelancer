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

		[XmlElement("value")]
		public AlchemyValue Value;
		

		public override string ToString()
		{
			return $"[{Name}: {Value}]";
		}
	}
}

