using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillDB
{
    public static List<Skill> skills = new List<Skill>() 
    {
        new Skill(0, "Rain Of Arrows", "Rain of deadly arrows floods a battlefield.", 7, "arrowFlights", 1, 0, Skill.TargetType.All, Effect.EffectType.Other),
        new Skill(1, "Holy Armour", "Gives an armour to your pawns, making them immune to any damage.", 8, "holyArmour", 1, 3, Skill.TargetType.Allies, Effect.EffectType.DamageImmune),
        new Skill(2, "Weather Control", "Gives you an ability to change weather and ground condition on the map.", 5, "weatherControl", 2, 3, Skill.TargetType.All, Effect.EffectType.Other),
        new Skill(3, "Mud Trap", "Slows every pawn in the selected fields.", 6, "mudTrap", 2, 3, Skill.TargetType.All, Effect.EffectType.Slow),
        new Skill(4, "Thunder Strike", "Powerful thunder strikes a ground on selected field anihilating a unit and giving acceleration buff to units on adjacents fields.",6, "thunderStrike", 1, 2, Skill.TargetType.All, Effect.EffectType.Other),
        new Skill(5, "Poisonous Arrow", "Posion every pawn slowing its movement.", 7, "poisonArrow", 1, 4, Skill.TargetType.All, Effect.EffectType.Slow),
        new Skill(6, "Cavalry Charge", "Paladins cavalry charge with a 50 % chance to kill enemy pawns.", 6, "cavalryCharge", 3, 0, Skill.TargetType.Enemies, Effect.EffectType.Other, 0.5f),
        new Skill(7, "Bloody Ambush", "Under the cover of the night assassins ambush enemies with a 75 % chance of succeeding.", 6, "bloodKnife", 1, 0, Skill.TargetType.Enemies, Effect.EffectType.Other, 0.75f),
        new Skill(8, "Purification Blessing", "Mighty blessing which purify allies by giving them immune to slows and stuns.", 7, "purificationSwirl", 1, 4, Skill.TargetType.Allies, Effect.EffectType.Other)
    };
	
}
