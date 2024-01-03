﻿using System;

namespace Ambermoon.Data
{
    [Flags]
    public enum ItemSlotFlags : byte
    {
        None = 0,
        Identified = 0x01, // only used for magic items
        Broken = 0x02,
        Cursed = 0x04, // this is only used for equip slots where a cursed item was equipped
        Locked = 0x08 // Ambermoon Advanced only, can't drag the item
    }
}
