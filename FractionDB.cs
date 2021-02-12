using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractionDB
{
    static List<Fraction> fractions = new List<Fraction>() 
    {
        new Fraction("Paladins", Fraction.RaceType.Human, new int[] {1, 6}),
        new Fraction("Mages", Fraction.RaceType.Human, new int[] { 4, 8 }),
        new Fraction("Hunters", Fraction.RaceType.Human, new int[] { 0, 5 }),
        new Fraction("Rogues", Fraction.RaceType.Human, new int[] { 3, 7 })
    };
	public static Fraction FindByName(string name)
    {
        foreach (Fraction f in fractions)
        {
            if (f.name == name) return f;
        }
        return null;
    }
    public static List<string> GetFractionsNames()
    {
        List<string> names = new List<string>();

        foreach (var item in fractions)
        {
            names.Add(item.name);
        }

        return names;
    }
}
