﻿using System;

namespace Ambermoon.UI
{
    public enum Window
    {
        MapView,
        Inventory,
        Stats,
        Chest,
        Merchant,
        Event,
        Riddlemouth,
        Conversation,
        Battle,
        BattleLoot,
        BattlePositions,
        // TODO ...
    }

    public struct WindowInfo
    {
        public Window Window;
        public object[] WindowParameters; // party member index, chest event, etc
        public bool Closable => Window != Window.Battle; // TODO: add more (windows without exit button)
    }
}
