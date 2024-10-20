// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.IO;
using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale
{
	[XmlRoot("ale")]
	public class AleFile : UtfFile
	{
		[XmlElement("aleEffectLib")]
		public ALEffectLib FxLib;

		[XmlElement("alchemyNodeLibrary")]
		public AlchemyNodeLibrary NodeLib;

		[XmlIgnore]
        public string Path;

		public AleFile(string file, Stream stream) : this(parseFile(file, stream))
        {
            Path = file;
        }

		public AleFile (IntermediateNode root)
		{
			//TODO: This is ugly
			foreach (var node in root) {
				switch (node.Name.ToLowerInvariant ()) {
				case "aleffectlib":
					FxLib = new ALEffectLib ((node as IntermediateNode) [0] as LeafNode);
					break;
				case "alchemynodelibrary":
					NodeLib = new AlchemyNodeLibrary ((node as IntermediateNode) [0] as LeafNode);
					break;
				default:
					throw new NotImplementedException (node.Name);
				}
			}
            Path = "[utf]";
        }

		public AleFile()
		{

		}
	}
}

