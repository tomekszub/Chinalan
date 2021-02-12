using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Fraction
{
    public string name;
	public enum RaceType : byte
    {
        Human,
        Orc,
        Elf,
        DarkElf,
        Dwarf,
        Undead
    }
    public RaceType race;
    public int[] skills;
    public Fraction(string Name, RaceType Race, int[] Skills)
    {
        name = Name;
        race = Race;
        skills = Skills;
    }
}
