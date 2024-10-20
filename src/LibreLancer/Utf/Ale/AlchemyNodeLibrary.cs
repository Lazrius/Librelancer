// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale
{
	public class AlchemyNodeLibrary
	{
		enum AleTypes {
			Boolean = 0x001,
			Integer = 0x002,
			Float = 0x003,
			Name = 0x103,
			IPair = 0x104,
			Transform = 0x105,
			FloatAnimation = 0x200,
			ColorAnimation = 0x201,
			CurveAnimation = 0x202
		}

		[XmlAttribute("version")]
		public float Version;

		[XmlElement("Node")]
		public List<AlchemyNode> Nodes = new();

		public AlchemyNodeLibrary()
		{

		}
		public AlchemyNodeLibrary (LeafNode utfleaf)
		{
			using (var reader = new BinaryReader (utfleaf.DataSegment.GetReadStream())) {
				Version = reader.ReadSingle ();
				var nodeCount = reader.ReadInt32 ();
				for (var nc = 0; nc < nodeCount; nc++) {
					var nameLen = reader.ReadUInt16 ();
					var nodeName = Encoding.ASCII.GetString (reader.ReadBytes (nameLen)).TrimEnd ('\0');
					reader.BaseStream.Seek(nameLen & 1, SeekOrigin.Current); //padding
					var node = new AlchemyNode
                    {
                        Name = nodeName,
                        CRC = CrcTool.FLAleCrc(nodeName)
                    };

                    uint id, crc;
					while (true)
                    {
						id = reader.ReadUInt16 ();
						if (id == 0)
                        {
                            break;
                        }

                        AleTypes type = (AleTypes)(id & 0x7FFF);
						crc = reader.ReadUInt32();

                        if (!AleCrc.FxCrc.TryGetValue (crc, out var efname))
                        {
							efname = $"CRC: 0x{crc:X}";
						}
                        AlchemyValue value = null;
						switch (type) {
						case AleTypes.Boolean:
							value = new AlchemyBool() { Value = (id & 0x8000) != 0 };
							break;
						case AleTypes.Integer:
							value = new AlchemyInteger() { Value = reader.ReadInt32() };
							break;
						case AleTypes.Float:
							value = new AlchemyFloat() { Value = reader.ReadSingle () };
							break;
						case AleTypes.Name:
							var valLen = reader.ReadUInt16 ();
                            if (valLen != 0)
                            {
                                value = new AlchemyString()
                                    { Value = Encoding.ASCII.GetString(reader.ReadBytes(valLen)).TrimEnd('\0') };
                            }

                            reader.BaseStream.Seek(valLen & 1, SeekOrigin.Current); //padding
							break;
						case AleTypes.IPair:
                            value = new AlchemyBlendInfo() { SrcFactor = (BlendFactor)reader.ReadUInt32(), DstFactor = (BlendFactor)reader.ReadUInt32() };
							break;
						case AleTypes.Transform:
							value = new AlchemyTransform (reader);
							break;
						case AleTypes.FloatAnimation:
							value = new AlchemyFloatAnimation (reader);
							break;
						case AleTypes.ColorAnimation:
							value = new AlchemyColorAnimation (reader);
							break;
						case AleTypes.CurveAnimation:
							value = new AlchemyCurveAnimation (reader);
							break;
						default:
							throw new InvalidDataException ("Invalid ALE Type: 0x" + (id & 0x7FFF).ToString ("x"));
						}

						node.Parameters.Add(new AleParameter () { Name = efname, Value = value });
					}

                    if (node.TryGetParameter("Node_Name", out var temp))
					{
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        var nn = (temp.Value as AlchemyString)!.Value;
						node.CRC = CrcTool.FLAleCrc(nn);
					}
					Nodes.Add(node);
				}
			}
		}
	}
}

