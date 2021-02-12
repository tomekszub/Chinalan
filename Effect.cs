[System.Serializable]
public class Effect
{
    // 
    public enum EffectType : byte
    {
        StunImmune,
        Immune,
        DamageImmune,
        SlowImmune,
        AccelerationImmune,  // koniec immunow index 4 
        GrassSpeed,
        SandSpeed,
        SnowSpeed,
        AdditionalMovement,   // np. +1 do oczekiwanego ruchu
        Stun,
        Slow,
        Acceleration,
        Other  // uzywany tylko podczas tworzenia skila dla ujednolicenia dodawania efektow
    }
    public EffectType typeOfEffect;
    /// <summary>
    /// The number of turns in which the effect will disappear
    /// </summary>
    public int turnsToDisappear;
    public bool immuneEffect;
    public Effect(EffectType TypeOfEffect, int TurnsToDisappear)
    {
        typeOfEffect = TypeOfEffect;
        turnsToDisappear = TurnsToDisappear;
        CheckIfImmune();
    }
    void CheckIfImmune()
    {
        if(typeOfEffect.ToString().Contains("Immune")) immuneEffect = true;
        else immuneEffect = false;
    }
}
