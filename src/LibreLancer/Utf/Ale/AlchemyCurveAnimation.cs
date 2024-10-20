﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
namespace LibreLancer.Utf.Ale;

public sealed class AlchemyCurveAnimation : AlchemyValue
{
    [XmlAttribute("easingType")]
    public EasingTypes Type;

    [XmlElement("Item")]
    public List<AlchemyCurve> Items;

    public AlchemyCurveAnimation()
    {

    }

    public AlchemyCurveAnimation (BinaryReader reader)
    {
        Type = (EasingTypes)reader.ReadByte ();
        int scount = reader.ReadByte ();
        Items = new List<AlchemyCurve> (scount);
        for (var i = 0; i < scount; i++) {
            var cpkf = new AlchemyCurve ();
            cpkf.SParam = reader.ReadSingle ();
            cpkf.Value = reader.ReadSingle ();
            var loop = reader.ReadUInt16 ();
            cpkf.Flags = (LoopFlags)loop;
            var lcnt = reader.ReadUInt16 ();
            if (loop != 0 || lcnt != 0) {
                var l = new List<CurveKeyframe> (lcnt);
                for (var j = 0; j < lcnt; j++) {
                    l.Add (new CurveKeyframe () {
                        Time = reader.ReadSingle(),
                        Value = reader.ReadSingle(),
                        End = reader.ReadSingle(),
                        Start = reader.ReadSingle(),
                    });
                }
                cpkf.Keyframes = l;
            }
            Items.Add (cpkf);
        }
    }

    public bool Animates
    {
        get
        {
            return !(Items.Count == 1 && !Items[0].Animates);
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
        AlchemyCurve c1 = null, c2 = null;
        for (var i = 0; i < Items.Count - 1; i++) {
            if (sparam >= Items [i].SParam && sparam <= Items [i + 1].SParam) {
                c1 = Items [i];
                c2 = Items [i + 1];
                if (i + 2 < Items.Count && (sparam >= Items[i + 1].SParam && sparam <= Items[i + 2].SParam))
                {
                    //go one more for duplicates
                    c1 = Items[i + 1];
                    c2 = Items[i + 2];
                }
                break;
            }
        }
        //We're at the end
        if (c1 == null) {
            return Items [Items.Count - 1].GetValue(time);
        }
        //Interpolate between SParams
        if (Math.Abs(c1.SParam - c2.SParam) < float.Epsilon) return c2.GetValue(time);
        var v1 = c1.GetValue (time);
        var v2 = c2.GetValue (time);
        return Easing.Ease (Type, sparam, c1.SParam, c2.SParam, v1, v2);
    }
}
