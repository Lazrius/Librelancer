// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale
{
	public class AlchemyNode
	{
		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("crc")]
		public uint CRC;

		[XmlElement("Param")]
		public List<AleParameter> Parameters = new();

		public AlchemyNode ()
		{
		}

		public override string ToString ()
		{
			return Name;
		}

		public bool TryGetParameter(string name, out AleParameter parameter)
		{
			parameter = null;
			var nm = name.ToUpperInvariant ();
			foreach (var p in Parameters) {
				if (p.Name.ToUpperInvariant () == nm) {
					parameter = p;
					return true;
				}
			}
			return false;
		}
	}
}

