public class Stats
{
    private string name;
    private int kills;
    private int deaths;
    private int onesRolled;
    private int twosRolled;
    private int threesRolled;
    private int foursRolled;
    private int fivesRolled;
    private int sixsRolled;
    private float gold;

    public int Kills { get => kills; set => kills = value; }
    public int Deaths { get => deaths; set => deaths = value; }
    public int OnesRolled { get => onesRolled; private set => onesRolled = value; }
    public int TwosRolled { get => twosRolled; private set => twosRolled = value; }
    public int ThreesRolled { get => threesRolled; private set => threesRolled = value; }
    public int FoursRolled { get => foursRolled; private set => foursRolled = value; }
    public int FivesRolled { get => fivesRolled; private set => fivesRolled = value; }
    public int SixsRolled { get => sixsRolled; private set => sixsRolled = value; }
    public float Gold { get => gold; set => gold = value; }
    public string Name { get => name; private set => name = value; }

    public Stats(string Name, int Kills, int Deaths, int OnesRolled, int TwosRolled, int ThreesRolled, int FoursRolled, int FivesRolled, int SixsRolled, float Gold)
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
        gold = Gold;
    }
    public void AddRolledNumber(int number)
    {
        switch (number)
        {
            case 1: OnesRolled++; break;
            case 2: TwosRolled++; break;
            case 3: ThreesRolled++; break;
            case 4: FoursRolled++; break;
            case 5: FivesRolled++; break;
            case 6: SixsRolled++; break;
            default:
                break;
        }
    }
}
