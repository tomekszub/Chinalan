using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    private int owner;
    private int currFieldIndex;
    private bool isSafe = false;
    public List<Effect> effects = new List<Effect>();
    //public bool isAbleToMove = true;
    Vector3 newPosition;
    private float startTime;
    float speed = 2.5F;
    // Total distance between the markers.
    private float journeyLength;
    private bool isMoving;
    public bool IsMoving { get => isMoving; private set => isMoving = value; }
    public int Owner { get => owner; set => owner = value; }
    public int CurrFieldIndex { get => currFieldIndex; set => currFieldIndex = value; }
    public bool IsSafe { get => isSafe; set => isSafe = value; }

    PawnStats pawnStats;

    

    void Start ()
    {
        newPosition = transform.position;
        pawnStats = new PawnStats();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (newPosition != transform.position)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, newPosition, fracJourney);
        }
        else
        {
            isMoving = false;
        }
            
    }

    public void SetNewPosition(Vector3 pos)
    {
        isMoving = true;
        newPosition = new Vector3(pos.x, pos.y + 0.5f, pos.z);
        startTime = Time.time;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(transform.position, newPosition);
    }

    public bool CheckHasEffect(Effect.EffectType effectType)
    {
        foreach (var effect in effects)
        {
            if (effect.typeOfEffect == effectType) return true;
        }
        return false;
    }

    public float GetMovementCost(Pole.TerrainType terrainType)
    {
        float cost = 0;
        switch (terrainType)
        {
            case Pole.TerrainType.Base:
                cost = pawnStats.terrainTypeMovements[0];
                break;
            case Pole.TerrainType.Grass:
                cost = pawnStats.terrainTypeMovements[1];
                if (CheckHasEffect(Effect.EffectType.GrassSpeed)) cost /= 2;
                break;
            case Pole.TerrainType.Sand:
                cost = pawnStats.terrainTypeMovements[2];
                if (CheckHasEffect(Effect.EffectType.SandSpeed)) cost /= 2;
                break;
            case Pole.TerrainType.Snow:
                cost = pawnStats.terrainTypeMovements[3];
                if (CheckHasEffect(Effect.EffectType.SnowSpeed)) cost /= 2;
                break;
            default:
                break;
        }
        return cost;
    }

    /*public void ApplyEffect(Effect e)
    {
        if (e.typeOfEffect == Effect.EffectType.Acceleration && !pawnStats.immunes[0])
        {
            for (int i = 1; i < pawnStats.terrainTypeMovements.Length; i++)
            {
                pawnStats.terrainTypeMovements[i] /= 2;
            }
        }
        else if (e.typeOfEffect == Effect.EffectType.AccelerationImmune)
        {
            pawnStats.immunes[0] = true;
        }
        else if (e.typeOfEffect == Effect.EffectType.AdditionalMovement)
        {
            pawnStats.AdditionalMove += 1;
        }
        else if (e.typeOfEffect == Effect.EffectType.DamageImmune)
        {
            pawnStats.immunes[1] = true;
        }
        else if (e.typeOfEffect == Effect.EffectType.GrassSpeed && !pawnStats.immunes[0])
        {
            pawnStats.terrainTypeMovements[1] /= 2;
        }
        else if (e.typeOfEffect == Effect.EffectType.Immune)
        {
            for (int i = 1; i < pawnStats.immunes.Length; i++)
            {
                pawnStats.immunes[i] = true;
            }
        }
        else if (e.typeOfEffect == Effect.EffectType.SandSpeed && !pawnStats.immunes[0])
        {
            pawnStats.terrainTypeMovements[2] /= 2;
        }
        else if (e.typeOfEffect == Effect.EffectType.Slow && !pawnStats.immunes[2])
        {
            for (int i = 1; i < pawnStats.terrainTypeMovements.Length; i++)
            {
                pawnStats.terrainTypeMovements[i] *= 2;
            }
        }
        else if (e.typeOfEffect == Effect.EffectType.SlowImmune)
        {
            pawnStats.immunes[2] = true;
        }
        else if (e.typeOfEffect == Effect.EffectType.SnowSpeed && !pawnStats.immunes[0])
        {
            pawnStats.terrainTypeMovements[3] /= 2;
        }
        else if (e.typeOfEffect == Effect.EffectType.Stun && !pawnStats.immunes[3])
        {
            isAbleToMove = false;
        }
        else if (e.typeOfEffect == Effect.EffectType.StunImmune)
        {
            pawnStats.immunes[3] = true;
        }
    }
    public void DeleteEffect(Effect e)
    {
        if (e.typeOfEffect == Effect.EffectType.Acceleration)
        {
            for (int i = 1; i < pawnStats.terrainTypeMovements.Length; i++)
            {
                pawnStats.terrainTypeMovements[i] *= 2;
            }
        }
        else if (e.typeOfEffect == Effect.EffectType.AccelerationImmune)
        {
            pawnStats.immunes[0] = false;
        }
        else if (e.typeOfEffect == Effect.EffectType.AdditionalMovement)
        {
            pawnStats.AdditionalMove -= 1;
        }
        else if (e.typeOfEffect == Effect.EffectType.DamageImmune)
        {
            pawnStats.immunes[1] = false;
        }
        else if (e.typeOfEffect == Effect.EffectType.GrassSpeed)
        {
            pawnStats.terrainTypeMovements[1] *= 2;
        }
        else if (e.typeOfEffect == Effect.EffectType.Immune)
        {
            for (int i = 1; i < pawnStats.immunes.Length; i++)
            {
                pawnStats.immunes[i] = false;
            }
        }
        else if (e.typeOfEffect == Effect.EffectType.SandSpeed)
        {
            pawnStats.terrainTypeMovements[2] *= 2;
        }
        else if (e.typeOfEffect == Effect.EffectType.Slow)
        {
            for (int i = 1; i < pawnStats.terrainTypeMovements.Length; i++)
            {
                pawnStats.terrainTypeMovements[i] /= 2;
            }
        }
        else if (e.typeOfEffect == Effect.EffectType.SlowImmune)
        {
            pawnStats.immunes[2] = false;
        }
        else if (e.typeOfEffect == Effect.EffectType.SnowSpeed)
        {
            pawnStats.terrainTypeMovements[3] *= 2;
        }
        else if (e.typeOfEffect == Effect.EffectType.Stun)
        {
            isAbleToMove = true;
        }
        else if (e.typeOfEffect == Effect.EffectType.StunImmune)
        {
            pawnStats.immunes[3] = false;
        }
    }
    public void UpdateEffect(Effect e)
    {
        DeleteEffect(e);
        ApplyEffect(e);
    }*/

}
