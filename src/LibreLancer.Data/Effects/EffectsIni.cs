﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Linq;
using LibreLancer.Data.IO;
using LibreLancer.Ini;
namespace LibreLancer.Data.Effects
{
    public class EffectsIni : IniFile
    {
        [Section("viseffect")]
        public List<VisEffect> VisEffects = new List<VisEffect>();
        [Section("beamspear")]
        public List<BeamSpear> BeamSpears = new List<BeamSpear>();
        [Section("beambolt")]
        public List<BeamBolt> BeamBolts = new List<BeamBolt>();
        [Section("effect")]
        public List<Effect> Effects = new List<Effect>();
        [Section("effecttype")]
        public List<EffectType> EffectTypes = new List<EffectType>();
        [Section("effectlod")]
        public List<EffectLOD> EffectLODs = new List<EffectLOD>();

        public void AddIni(string ini, FileSystem vfs) => ParseAndFill(ini, vfs);
	}
}
