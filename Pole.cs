using System;
using UnityEngine;

public class Pole : MonoBehaviour
{
    public int ID;
    public bool isSafehouse;
    Renderer thisRenderer;
    Color originalColor;
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
	// Use this for initialization
	void Awake ()
    {
        thisRenderer = GetComponent<Renderer>();
        originalColor = thisRenderer.material.color;
    }
	public void SetID(int id)
    {
        ID = id;
    }
    public void BackToOriginalColor()
    {
        thisRenderer.material.color = originalColor;
    }
}
