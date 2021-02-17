using UnityEngine;
using System;

public abstract class PointOfInterest : MonoBehaviour
{
    protected GameManager gm;
    public enum PointOfInterestType
    {
        MiningCamp
    }
    public static readonly int pointOfInterestTypeCount = Enum.GetNames(typeof(PointOfInterestType)).Length;
    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public virtual void GetArrivedBonus(int player)
    {
        if (IsPlayerValid(player))
            return;
    }
    public virtual void GetStayingBonus(int player)
    {
        if (IsPlayerValid(player))
            return;
    }
    bool IsPlayerValid(int player)
    {
        return player > 3 || player < 0;
    }
}
