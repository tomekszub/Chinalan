public class Stats
{
    private string name;
    private int id;
    private int kills;
    private int deaths;
    private int onesRolled;
    private int twosRolled;
    private int threesRolled;
    private int foursRolled;
    private int fivesRolled;
    private int sixsRolled;
    private float gold;

    public delegate void GoldChangedHandler(int playerId);
    public static GoldChangedHandler OnGoldChanged;
    public int Kills { get => kills; set => kills = value; }
    public int Deaths { get => deaths; set => deaths = value; }
    public int OnesRolled { get => onesRolled; private set => onesRolled = value; }
    public int TwosRolled { get => twosRolled; private set => twosRolled = value; }
    public int ThreesRolled { get => threesRolled; private set => threesRolled = value; }
    public int FoursRolled { get => foursRolled; private set => foursRolled = value; }
    public int FivesRolled { get => fivesRolled; private set => fivesRolled = value; }
    public int SixsRolled { get => sixsRolled; private set => sixsRolled = value; }
    public float Gold 
    { 
        get => gold;
        set
        {
            if(OnGoldChanged != null)
                OnGoldChanged(Id);
            gold = value;
        }
    }
    public string Name { get => name; private set => name = value; }
    public int Id { get => id; set => id = value; }

    public Stats(string Name, int Id, int Kills, int Deaths,
        int OnesRolled, int TwosRolled, int ThreesRolled, int FoursRolled, int FivesRolled, int SixsRolled, float Gold)
    {
        this.Name = Name;
        this.Id = Id;
        this.Kills = Kills;
        this.Deaths = Deaths;
        this.OnesRolled = OnesRolled;
        this.TwosRolled = TwosRolled;
        this.ThreesRolled = ThreesRolled;
        this.FoursRolled = FoursRolled;
        this.FivesRolled = FivesRolled;
        this.SixsRolled = SixsRolled;
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
