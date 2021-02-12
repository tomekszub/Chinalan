using System;
using UnityEngine;

public class Pole : MonoBehaviour
{
    public int ID;
    public bool isSafehouse;
    //public Pawn stationedPawn;
    Color originalColor;
    public enum TerrainType
    {
        // po dodaniu typu trzba go obsluzyc w switchu w klasie pawn oraz nizej w metodzie ChangeTerrainType
        //(nie zbyt elegancko lecz aktualnie brak alternatywy)
        Base,
        Grass,
        Sand,
        Snow
    }
    public TerrainType terrainType = TerrainType.Grass;
    public static readonly int terrainTypeCount = Enum.GetNames(typeof(TerrainType)).Length;
	// Use this for initialization
	void Awake ()
    {
        
        //if(isSafehouse) ChangeTerrainType(TerrainType.Base);
        //else ChangeTerrainType(TerrainType.Grass);
    }
	public void SetStartingValues(int id, bool isSafehouse, TerrainType terrainType)
    {
        ID = id;
        this.isSafehouse = isSafehouse;
        ChangeTerrainType(terrainType);
    }
	public void ChangeTerrainType(TerrainType type)
    {
        terrainType = type;
    }
    public void BackToOriginalColor()
    {
        GetComponent<Renderer>().material.color = originalColor;
    }
}
