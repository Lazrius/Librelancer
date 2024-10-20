// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LibreLancer.Utf.Ale
{
	public sealed class AlchemyFloats
	{
		[XmlAttribute("sparam")]
		public float SParam;

		[XmlAttribute("type")]
		public EasingTypes Type;

		[XmlArray("Data")]
		[XmlArrayItem("Value")]
		public AlchemyKeyFrameValue[] Data;

		public AlchemyFloats ()
		{
		}

		public float GetValue(float time) {
			//Only have one keyframe? Just return it.
			if (Data.Length == 1) {
				return Data [0].Value;
			}
			//Locate the keyframes to interpolate between
			var t1 = float.NegativeInfinity;
			float t2 = 0, v1 = 0, v2 = 0;
			for (var i = 0; i < Data.Length - 1; i++)
            {
                if (!(time >= Data[i].Keyframe) || !(time <= Data[i + 1].Keyframe))
                {
                    continue;
                }

                t1 = Data [i].Keyframe;
                t2 = Data [i + 1].Keyframe;
                v1 = Data [i].Value;
                v2 = Data [i + 1].Value;
                break;
            }

            //Time wasn't between any values. Return max.
            return float.IsNegativeInfinity(t1) ? Data [^1].Value : Easing.Ease(Type,time, t1, t2, v1, v2);
        }

        public float GetMax(bool abs)
        {
            return Data.Select(i => abs ? Math.Abs(i.Value) : i.Keyframe).Prepend(0).Max();
        }
	}
}

