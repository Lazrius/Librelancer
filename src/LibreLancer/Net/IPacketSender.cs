// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using LibreLancer.Net.Protocol;

namespace LibreLancer.Net
{
    public interface IPacketSender
    {
        int MaxSequencedSize { get; }
        void SendPacket(IPacket packet, PacketDeliveryMethod method);
    }
}
