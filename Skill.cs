using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Skill
{
    public enum TargetType : byte
    {
        All,
        Enemies,
        Allies
    }
    public int id;
    public string name;
    public string description;
    public int cooldown;
    public int currCooldown;
    public string iconName;
    public int boundaryRange; //zasieg dzialania poza miejscem wskazanym przez gracza
    public int duration;
    public TargetType targetLimit;
    public Effect.EffectType effectType;
    /// <summary>
    /// From 0.0 to 1.0
    /// </summary>
    public float successChance;
    public Skill(int Id, string Name, string Description, int Cooldown, string IconName, int BoundaryRange, int Duration, TargetType TargetLimit, Effect.EffectType EffectType, float SuccessChance = 1.0f)
    {
        id = Id;
        name = Name;
        description = Description;
        currCooldown = 0;
        cooldown = Cooldown;
        iconName = IconName;
        boundaryRange = BoundaryRange;
        duration = Duration;
        targetLimit = TargetLimit;
        effectType = EffectType;
        successChance = SuccessChance;
    }
    public Skill(Skill skill)
    {
        id = skill.id;
        name = skill.name;
        description = skill.description;
        currCooldown = 0;
        cooldown = skill.cooldown;
        iconName = skill.iconName;
        boundaryRange = skill.boundaryRange;
        duration = skill.duration;
        targetLimit = skill.targetLimit;
        effectType = skill.effectType;
        successChance = skill.successChance;
    }
	public void TurnOnCooldown()
    {
        currCooldown = cooldown;
    }
}
