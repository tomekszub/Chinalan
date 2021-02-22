using UnityEngine;
public class PreMatchSettings : MonoBehaviour
{
    public TMPro.TMP_Dropdown[] playerDropdowns;
    public TMPro.TMP_Text[] playerInputs;
	// Use this for initialization
	void Start ()
    {
        foreach (TMPro.TMP_Dropdown d in playerDropdowns)
        {
            d.AddOptions(FractionDB.GetFractionsNames());
        }
	}
	public void ApplySettings()
    {
        string[] temp = new string[8];
        for (int i = 0; i < 4; i++)
        {
            temp[i] = playerDropdowns[i].options[playerDropdowns[i].value].text;
        }
        for (int i = 4; i < 8; i++)
        {
            temp[i] = playerInputs[i - 4].text.Length <= 1 ? ("Player " + (i-3)) : playerInputs[i - 4].text;
        }
        
        GetComponent<GameManager>().StartGame(temp);
    }
}
