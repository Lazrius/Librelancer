// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale
{
	public sealed class AlchemyColorAnimation : AlchemyValue
	{
		[XmlElement("type")]
		public EasingTypes Type;

		[XmlArray("items")]
		[XmlArrayItem("item")]
		public List<AlchemyColors> Items = new();

		public AlchemyColorAnimation()
		{

		}

		public AlchemyColorAnimation(BinaryReader reader)
		{
			Type = (EasingTypes)reader.ReadByte ();
			int itemsCount = reader.ReadByte ();
			for (var fc = 0; fc < itemsCount; fc++) {
				var colors = new AlchemyColors
                {
                    SParam = reader.ReadSingle (),
                    Type = (EasingTypes)reader.ReadByte (),
                    Data = new AlchemyKeyFrameColor[reader.ReadByte ()]
                };

                for (var i = 0; i < colors.Data.Length; i++) {
					colors.Data [i] = new AlchemyKeyFrameColor
                    {
                        Keyframe = reader.ReadSingle(),
                        Color = new Color3f (reader.ReadSingle (), reader.ReadSingle (), reader.ReadSingle ())
                    };
				}
				Items.Add (colors);
			}
		}

		public Color3f GetValue(float sparam, float time)
		{
			//1 item, 1 value
			if (Items.Count == 1) {
				return Items [0].GetValue (time);
			}
			//Find 2 keyframes to interpolate between
			AlchemyColors c1 = null, c2 = null;
			for (var i = 0; i < Items.Count - 1; i++) {
				if (sparam >= Items [i].SParam && sparam <= Items [i + 1].SParam) {
					c1 = Items [i];
					c2 = Items [i + 1];
                    break;
                }
			}
			//We're at the end
			if (c1 == null) {
				return Items [Items.Count - 1].GetValue(time);
			}
			//Interpolate between SParams
			var v1 = c1.GetValue (time);
			var v2 = c2.GetValue (time);
			return Easing.EaseColorRGB (Type, sparam, c1.SParam, c2.SParam, v1, v2);
		}

		public override string ToString ()
		{
			return string.Format ("<Canim: Type={0}, Count={1}>",Type,Items.Count);
		}
	}
}

