// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale
{
	public class AlchemyNodeRef
	{
		[XmlAttribute("flag")]
		public uint Flag;

		[XmlAttribute("crc")]
		public uint CRC;

		[XmlAttribute("parent")]
		public uint Parent;

		[XmlAttribute("index")]
		public uint Index;

		public AlchemyNodeRef()
		{

		}

		public AlchemyNodeRef(uint flg, uint crc, uint parent, uint idx)
		{
			Flag = flg;
			CRC = crc;
			Parent = parent;
			Index = idx;
		}

		public bool IsAttachmentNode {
			get {
				return Flag == 1;
			}
		}
	}
}

