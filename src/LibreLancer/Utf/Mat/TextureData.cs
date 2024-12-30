// MIT License - Copyright (c) Malte Rupprecht
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package


using System;
using System.Collections.Generic;
using System.IO;
using LibreLancer.Graphics;

namespace LibreLancer.Utf.Mat
{
	public class TextureData
	{
        public static bool Bitch = false;

		public string Type;
		public string Texname;
        public ArraySegment<byte>? Data;
        public Texture Texture { get; set; }
		public Dictionary<int, byte[]> Levels;

        public TextureData()
        {
        }

		public TextureData (LeafNode node, string texname, bool isTgaMips)
		{
			this.Type = node.Name;
			this.Texname = texname;
            this.Data = node.DataSegment;
			if (isTgaMips)
				Levels = new Dictionary<int, byte[]>();
		}


        public ImageResource GetImageResource()
        {
            if (Data != null && Data.Value.Count > 0)
            {
                if (Type.Equals("mips", StringComparison.OrdinalIgnoreCase))
                    return new ImageResource(ImageType.DDS, Data.Value.ToArray());
                else if (Type.Equals("mipu", StringComparison.OrdinalIgnoreCase))
                    return new ImageResource(ImageType.LIF, Data.Value.ToArray());
                else if (Type.StartsWith("mip", StringComparison.OrdinalIgnoreCase))
                    return new ImageResource(ImageType.TGA, Data.Value.ToArray());
            }
            return null;
        }

		public void Initialize (RenderContext context)
		{
			if (Data != null && Data.Value.Count > 0 && Texture == null)
            {
				using (Stream stream = Data.Value.GetReadStream()) {
					if (Type.Equals ("mips", StringComparison.OrdinalIgnoreCase))
                    {
                        Texture = ImageLib.DDS.FromStream(context, stream);
					}
                    else if (Type.Equals("mipu", StringComparison.OrdinalIgnoreCase))
                    {
                        Texture = ImageLib.LIF.TextureFromStream(context, stream);
                    }
                    else if (Type.StartsWith ("mip", StringComparison.OrdinalIgnoreCase))
                    {
						var tga = ImageLib.TGA.TextureFromStream(context, stream, Levels != null);
                        if(tga == null) {
                            FLLog.Error("Mat","Texture " + Texname + "\\MIP0" + " is bad");
                            if (Bitch) throw new Exception("Your texture data is bad, fix it!\n" +
                                                           Texname + "\\MIP0 to be exact");
                            Texture = null;
                            return;
                        }
						if (Levels != null)
						{
							foreach (var lv in Levels)
							{
								using (var s2 = new MemoryStream(lv.Value)) {
									ImageLib.TGA.TextureFromStream(context, s2, true, tga, lv.Key);
								}
							}
						}
						Texture = tga;
						Levels = null;
					}
                    else if (Type.Equals ("cube", StringComparison.OrdinalIgnoreCase))
                    {
						Texture = ImageLib.DDS.FromStream (context, stream);
					}
				}
			} else
				FLLog.Error ("Texture " + Texname, "data == null");
		}

		public void SetLevel(Node node)
		{
			var n = node as LeafNode;
			if (n == null)
				throw new Exception("Invalid node in TextureData MIPS " + node.Name);
			var name = n.Name.Trim();
			if (!name.StartsWith("mip", StringComparison.OrdinalIgnoreCase))
				throw new Exception("Invalid node in TextureData MIPS " + node.Name);
			var mipLevel = int.Parse(name.Substring(3));
			if (mipLevel == 0)
				return;
			Levels.Add(mipLevel, n.ByteArrayData);
		}

		public override string ToString ()
		{
			return Type;
		}
	}
}
