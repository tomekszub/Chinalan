public class Stats
{
    public string name;
    public int kills;
    public int deaths;
    public int onesRolled;
    public int twosRolled;
    public int threesRolled;
    public int foursRolled;
    public int fivesRolled;
    public int sixsRolled;
    public Stats(string Name, int Kills, int Deaths, int OnesRolled, int TwosRolled, int ThreesRolled, int FoursRolled, int FivesRolled, int SixsRolled)
    {
        name = Name;
        kills = Kills;
        deaths = Deaths;
        onesRolled = OnesRolled;
        twosRolled = TwosRolled;
        threesRolled = ThreesRolled;
        foursRolled = FoursRolled;
        fivesRolled = FivesRolled;
        sixsRolled = SixsRolled;
    }
    public void AddRolledNumber(int number)
    {
        switch (number)
        {
            case 1: onesRolled++; break;
            case 2: twosRolled++; break;
            case 3: threesRolled++; break;
            case 4: foursRolled++; break;
            case 5: fivesRolled++; break;
            case 6: sixsRolled++; break;
            default:
                break;
        }
    }
    public void SuccesfullKill()
    {
        kills++;
    }
    public void PawnDestroyed()
    {
        deaths++;
    }
}
