using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Player
{
    public int pawnsInGame;
    public Fraction fractionType;
    public List<Skill> skills;
    public Player(int PawnsInGame, Fraction FractionType)
    {
        pawnsInGame = PawnsInGame;
        fractionType = FractionType;
        skills = new List<Skill>();
    }
	
}
