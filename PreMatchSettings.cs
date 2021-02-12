using UnityEngine;
using UnityEngine.UI;
public class PreMatchSettings : MonoBehaviour
{
    public GameObject[] playerPanels;
    public GameObject[] playerInputs;
    GameManager gm;
	// Use this for initialization
	void Start ()
    {
        gm = GetComponent<GameManager>();
        foreach (GameObject go in playerPanels)
        {
            go.GetComponentInChildren<Dropdown>().AddOptions(FractionDB.GetFractionsNames());
        }
	}
	public void ApplySettings()
    {
        string[] temp = new string[8];
        for (int i = 0; i < 4; i++)
        {
            temp[i] = playerPanels[i].GetComponentInChildren<Dropdown>().options[playerPanels[i].GetComponentInChildren<Dropdown>().value].text;
        }
        for (int i = 4; i < 8; i++)
        {
            temp[i] = playerInputs[i - 4].GetComponent<Text>().text == "" ? ("Player " + (i-3)) : playerInputs[i - 4].GetComponent<Text>().text;
        }
        gm.StartGame(temp);
    }
}
