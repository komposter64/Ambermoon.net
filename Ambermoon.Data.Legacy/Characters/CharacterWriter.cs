﻿using Ambermoon.Data.Legacy.Serialization;
using Ambermoon.Data.Serialization;
using System;

namespace Ambermoon.Data.Legacy.Characters
{
    public abstract class CharacterWriter
    {
        internal void WriteCharacter(Character character, IDataWriter dataWriter)
        {
            dataWriter.WriteEnumAsByte(character.Type);
            dataWriter.WriteEnumAsByte(character.Gender);
            dataWriter.WriteEnumAsByte(character.Race);
            dataWriter.WriteEnumAsByte(character.Class);
            dataWriter.WriteEnumAsByte(character.SpellMastery);
            dataWriter.Write(character.Level);
            dataWriter.Write(character.NumberOfFreeHands);
            dataWriter.Write(character.NumberOfFreeFingers);
            dataWriter.WriteEnumAsByte(character.SpokenLanguages);
            dataWriter.Write(character.PortraitIndex);
            dataWriter.Write(GetIfMonster<ushort>(character, monster => monster.CombatGraphicIndex, 0));
            dataWriter.Write(character.UnknownBytes13); // Unknown
            dataWriter.WriteEnumAsByte(character.SpellTypeImmunity);
            dataWriter.Write(character.AttacksPerRound);
            dataWriter.Write(GetIfMonster<byte>(character, monster => (byte)monster.MonsterFlags, 0));
            dataWriter.WriteEnumAsByte(character.Element);
            dataWriter.Write(character.SpellLearningPoints);
            dataWriter.Write(character.TrainingPoints);
            dataWriter.Write(character.Gold);
            dataWriter.Write(character.Food);
            dataWriter.Write(character.UnknownWord28); // Unknown
            dataWriter.WriteEnumAsWord(character.Ailments);
            dataWriter.Write(GetIfMonster<ushort>(character, monster => monster.DefeatExperience, 0));
            dataWriter.Write(character.UnknownWord34); // Unknown
            dataWriter.Write(GetIfPartyMember<ushort>(character, member => member.MarkOfReturnX, 0));
            dataWriter.Write(GetIfPartyMember<ushort>(character, member => member.MarkOfReturnY, 0));
            dataWriter.Write(GetIfPartyMember<ushort>(character, member => member.MarkOfReturnMapIndex, 0));
            foreach (var attribute in character.Attributes) // Note: this includes Age and the 10th unused attribute
            {
                dataWriter.Write((ushort)attribute.CurrentValue);
                dataWriter.Write((ushort)attribute.MaxValue);
                dataWriter.Write((ushort)attribute.BonusValue);
                dataWriter.Write((ushort)attribute.Unknown); // Unknown
            }
            foreach (var ability in character.Abilities)
            {
                dataWriter.Write((ushort)ability.CurrentValue);
                dataWriter.Write((ushort)ability.MaxValue);
                dataWriter.Write((ushort)ability.BonusValue);
                dataWriter.Write((ushort)ability.Unknown); // Unknown
            }
            dataWriter.Write((ushort)character.HitPoints.CurrentValue);
            dataWriter.Write((ushort)character.HitPoints.MaxValue);
            dataWriter.Write((ushort)character.HitPoints.BonusValue);
            dataWriter.Write((ushort)character.SpellPoints.CurrentValue);
            dataWriter.Write((ushort)character.SpellPoints.MaxValue);
            dataWriter.Write((ushort)character.SpellPoints.BonusValue);
            dataWriter.Write((ushort)character.CombatDefense);
            dataWriter.Write((ushort)character.DisplayedDefense);
            dataWriter.Write((ushort)character.CombatAttack);
            dataWriter.Write((ushort)character.DisplayedAttack);
            dataWriter.Write((ushort)character.MagicAttack);
            dataWriter.Write((ushort)character.MagicDefense);
            dataWriter.Write(character.AttacksPerRoundPerLevel);
            dataWriter.Write(character.HitPointsPerLevel);
            dataWriter.Write(character.SpellPointsPerLevel);
            dataWriter.Write(character.SpellLearningPointsPerLevel);
            dataWriter.Write(character.TrainingPointsPerLevel);
            dataWriter.Write(character.UnknownWord236); // Unknown
            dataWriter.Write(character.ExperiencePoints);
            dataWriter.Write(character.LearnedHealingSpells);
            dataWriter.Write(character.LearnedAlchemisticSpells);
            dataWriter.Write(character.LearnedMysticSpells);
            dataWriter.Write(character.LearnedDestructionSpells);
            dataWriter.Write(character.LearnedSpellsType5);
            dataWriter.Write(character.LearnedSpellsType6);
            dataWriter.Write(character.LearnedSpellsType7);
            dataWriter.Write(character.TotalWeight);
            string serializedCharacterName = character.Name.Length == 16
                ? character.Name.Substring(0, 15) + "\0"
                : character.Name.PadRight(16, '\0');
            dataWriter.WriteWithoutLength(serializedCharacterName);

            if (character.Type != CharacterType.NPC)
            {
                // Equipment
                foreach (var equipmentSlot in Enum.GetValues<EquipmentSlot>())
                {
                    if (equipmentSlot != EquipmentSlot.None)
                        ItemSlotWriter.WriteItemSlot(character.Equipment.Slots[equipmentSlot], dataWriter);
                }

                // Inventory
                for (int i = 0; i < Inventory.Width * Inventory.Height; ++i)
                    ItemSlotWriter.WriteItemSlot(character.Inventory.Slots[i], dataWriter);
            }
        }

        T GetIfMonster<T>(Character character, Func<Monster, T> valueProvider, T nonMonsterValue)
        {
            return character is Monster monster ? valueProvider(monster) : nonMonsterValue;
        }

        T GetIfPartyMember<T>(Character character, Func<PartyMember, T> valueProvider, T nonPartyMemberValue)
        {
            return character is PartyMember member ? valueProvider(member) : nonPartyMemberValue;
        }
    }
}
