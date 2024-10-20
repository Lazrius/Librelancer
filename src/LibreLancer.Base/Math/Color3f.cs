﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Xml.Serialization;

namespace LibreLancer
{
	public struct Color3f : IEquatable<Color3f>
    {
        public bool Equals(Color3f other)
        {
            return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is Color3f other && Equals(other);
        }

        public static bool operator ==(Color3f left, Color3f right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color3f left, Color3f right)
        {
            return !left.Equals(right);
        }

        public static readonly Color3f White = new Color3f(1, 1, 1);
		public static readonly Color3f Black = new Color3f(0, 0, 0);

        [XmlAttribute("r")]
		public float R;

        [XmlAttribute("g")]
		public float G;

        [XmlAttribute("b")]
		public float B;

        public Color3f()
        {
            R = 0.0f;
            G = 0.0f;
            B = 0.0f;
        }

		public Color3f(float r, float g, float b)
		{
			R = r;
			G = g;
			B = b;
		}

		public Color3f(Vector3 val) : this(val.X, val.Y, val.Z) {}
        public Color4 ToColor4() => new Color4(R, G, B, 1.0f);

		public override string ToString ()
		{
			return string.Format ("[R:{0}, G:{1}, B:{2}]", R, G, B);
		}

		public static Color3f operator *(Color3f a, Color3f b)
		{
			return new Color3f(
				a.R * b.R,
				a.G * b.G,
				a.B * b.B
			);
		}

        public static Color3f operator +(Color3f a, Color3f b)
        {
            return new Color3f(
                a.R + b.R,
                a.G + b.G,
                a.B + b.B
            );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B);
        }
    }
}

