﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale
{
	public sealed class AlchemyFloatAnimation : AlchemyValue
	{
		[XmlElement("type")]
		public EasingTypes Type;

		[XmlArray("items")]
		[XmlArrayItem("item")]
		public List<AlchemyFloats> Items = new();

		public AlchemyFloatAnimation()
		{

		}

		public AlchemyFloatAnimation (BinaryReader reader)
		{
			Type = (EasingTypes)reader.ReadByte ();
			int itemsCount = reader.ReadByte ();
			for (var fc = 0; fc < itemsCount; fc++)
            {
				var floats = new AlchemyFloats
                {
                    SParam = reader.ReadSingle (),
                    Type = (EasingTypes)reader.ReadByte (),
                    Data = new AlchemyKeyFrameValue[reader.ReadByte ()]
                };

                for (var i = 0; i < floats.Data.Length; i++)
                {
					floats.Data[i] = new AlchemyKeyFrameValue
                    {
                        Keyframe = reader.ReadSingle(),
                        Value = reader.ReadSingle ()
                    };
				}
				Items.Add(floats);
			}
		}

        public float GetMax(bool abs)
        {
            float max = 0;
            foreach (var item in Items)
            {
                var x = item.GetMax(abs);
                if (x > max) max = x;
            }
            return max;
        }
		public float GetValue(float sparam, float time)
		{
			//1 item, 1 value
			if (Items.Count == 1) {
				return Items [0].GetValue (time);
			}
			//Find 2 keyframes to interpolate between
			AlchemyFloats f1 = null, f2 = null;
			for (var i = 0; i < Items.Count - 1; i++) {
				if (sparam >= Items [i].SParam && sparam <= Items [i + 1].SParam) {
					f1 = Items [i];
					f2 = Items [i + 1];
                    break;
                }
			}
			//We're at the end
			if (f1 == null) {
				return Items [Items.Count - 1].GetValue(time);
			}
			//Interpolate between SParams
			var v1 = f1.GetValue (time);
			var v2 = f2.GetValue (time);
			return Easing.Ease (Type, sparam, f1.SParam, f2.SParam, v1, v2);
		}
		public override string ToString ()
		{
			return string.Format ("<Fanim: Type={0}, Count={1}>",Type,Items.Count);
		}
	}
}

