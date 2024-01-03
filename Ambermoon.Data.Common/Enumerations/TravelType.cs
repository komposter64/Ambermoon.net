﻿using System.ComponentModel;

namespace Ambermoon.Data.Enumerations
{
    /// <summary>
    /// The index here matches the travel gfx file index (0-based).
    /// Each travel gfx has 4 frames for directions Up, Right, Down and Left in that order.
    /// </summary>
    public enum TravelType : byte
    {
        Walk,
        Horse,
        Raft,
        Ship,
        MagicalDisc,
        Eagle,
        Fly, // never seen it but looks like flying with a cape like superman :D
        Swim,
        WitchBroom,
        SandLizard,
        SandShip,
        Wasp // Ambermoon Advanced only
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TravelTypeExtensions
    {
        public static bool UsesMapObject(this TravelType travelType) => travelType switch
        {
            TravelType.Horse => true,
            TravelType.Raft => true,
            TravelType.Ship => true,
            TravelType.SandLizard => true,
            TravelType.SandShip => true,
            _ => false
        };

        public static bool CanStandOn(this TravelType travelType) => travelType switch
        {
            TravelType.Raft => true,
            TravelType.Ship => true,
            TravelType.SandShip => true,
            _ => false
        };

        public static bool CanCampOn(this TravelType travelType) => travelType switch
        {
            TravelType.Walk => true,
            TravelType.Horse => true,
            TravelType.Raft => true,
            TravelType.Ship => true,
            TravelType.SandLizard => true,
            TravelType.SandShip => true,
            TravelType.MagicalDisc => true,
            TravelType.Fly => true,
            _ => false
        };

        public static bool IsStoppable(this TravelType travelType) => travelType switch
        {
            TravelType.Walk => false,
            TravelType.Swim => false,
            _ => true
        };

        public static bool BlockedByWater(this TravelType travelType) => travelType switch
        {
            TravelType.Horse => true,
            TravelType.SandLizard => true,
            _ => false
        };

        public static bool BlockedByTeleport(this TravelType travelType) => travelType switch
        {
            TravelType.Horse => true,
            TravelType.Raft => true,
            TravelType.Ship => true,
            TravelType.MagicalDisc => true,
            TravelType.SandLizard => true,
            TravelType.SandShip => true,
            _ => false
        };

        public static bool IgnoreEvents(this TravelType travelType) => travelType switch
        {
            TravelType.Eagle => true,
            TravelType.WitchBroom => true,
            TravelType.Fly => true,
            TravelType.Wasp => true,
            _ => false
        };

        public static bool IgnoreAutoPoison(this TravelType travelType) => travelType switch
        {
            TravelType.Raft => true,
            TravelType.Ship => true,
            TravelType.Eagle => true,
            TravelType.WitchBroom => true,
            TravelType.Fly => true,
            TravelType.SandShip => true,
            TravelType.Wasp => true,
            _ => false
        };

        public static Song TravelSong(this TravelType travelType) => travelType switch
        {
            TravelType.Walk => Song.Default,
            TravelType.Horse => Song.HorseIsNoDisgrace,
            TravelType.Raft => Song.RiversideTravellingBlues,
            TravelType.Ship => Song.Ship,
            TravelType.MagicalDisc => Song.CompactDisc,
            TravelType.Eagle => Song.WholeLottaDove,
            TravelType.Fly => Song.ChickenSoup,
            TravelType.Swim => Song.Default,
            TravelType.WitchBroom => Song.BurnBabyBurn,
            TravelType.SandLizard => Song.MellowCamelFunk,
            TravelType.SandShip => Song.PsychedelicDuneGroove,
            TravelType.Wasp => Song.WholeLottaDove,
            _ => Song.Default
        };

        public static uint AsStationaryImageIndex(this TravelType travelType) => travelType switch
        {
            TravelType.Horse => 0,
            TravelType.Raft => 1,
            TravelType.Ship => 2,
            TravelType.SandLizard => 3,
            TravelType.SandShip => 4,
            _ => throw new AmbermoonException(ExceptionScope.Application, $"Stationary image for travel type {travelType} does not exist.")
        };
    }
}
