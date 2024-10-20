// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale
{
	public class AlchemyColors
	{
		[XmlAttribute("sParam")]
		public float SParam;

		[XmlElement("type")]
		public EasingTypes Type;

		[XmlArray("data")]
		[XmlArrayItem("value")]
		public AlchemyKeyFrameColor[] Data;

		public AlchemyColors ()
        {
        }
		public Color3f GetValue(float time)
		{
			//Only have one keyframe? Just return it.
			if (Data.Length == 1) {
				return Data [0].Color;
			}
			//Locate the keyframes to interpolate between
			var t1 = float.NegativeInfinity;
			float t2 = 0;
			Color3f v1 = new Color3f(), v2 = new Color3f();
			for (var i = 0; i < Data.Length - 1; i++)
            {
                if (!(time >= Data[i].Keyframe) || !(time <= Data[i + 1].Keyframe))
                {
                    continue;
                }

                t1 = Data [i].Keyframe;
                t2 = Data [i + 1].Keyframe;
                v1 = Data [i].Color;
                v2 = Data [i + 1].Color;
                break;
            }

            //Time wasn't between any values. Return max, otherwise interpolate
            return float.IsNegativeInfinity(t1) ? Data [^1].Color : Easing.EaseColorRGB(Type,time, t1, t2, v1, v2);
        }
	}
}

