using UnityEngine;

public class MiningCamp : PointOfInterest
{
    [SerializeField]
    float goldPerTurn = 0.5f; 
    float accumulatedGold = 1;
    float maxaccumualtedGold = 5;
    public override void GetArrivedBonus(int player)
    {
        base.GetArrivedBonus(player);
        HandOverGold(player);
    }
    public override void GetStayingBonus(int player)
    {
        base.GetStayingBonus(player);
        HandOverGold(player);
    }
    void HandOverGold(int player)
    {
        gm.playerStats[player].Gold += accumulatedGold;
        accumulatedGold = 0;
    }
    void AccumulateGold()
    {
        if(accumulatedGold <= maxaccumualtedGold - goldPerTurn)
            accumulatedGold += goldPerTurn;
    }
    private void OnEnable()
    {
        gm.OnNextTurn += AccumulateGold;
    }
    private void OnDisable()
    {
        gm.OnNextTurn -= AccumulateGold;
    }

    
}
