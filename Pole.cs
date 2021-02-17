using System;
using UnityEngine;

public class Pole : MonoBehaviour
{
    public int ID;
    public bool isSafehouse;
    public PointOfInterest adjacentPointOfInterest = null;
    public enum TerrainType
    {
        // po dodaniu typu trzba go obsluzyc w switchu w klasie pawn
        Base,
        Grass,
        Sand,
        Snow
    }
    public TerrainType terrainType = TerrainType.Grass;
    public static readonly int terrainTypeCount = Enum.GetNames(typeof(TerrainType)).Length;
}
