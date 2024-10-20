// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Xml.Serialization;

namespace LibreLancer
{
	public struct CurveKeyframe
	{
		[XmlAttribute("time")]
		public float Time;

		[XmlAttribute("value")]
		public float Value;

		[XmlAttribute("end")]
        public float End;

		[XmlAttribute("start")]
        public float Start;
    }
}

