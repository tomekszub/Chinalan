using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [System.Serializable]
    public struct TutorialObject
    {
        public GameObject hintObject;
        public string hintText;
    }

    public GameObject tutorialCanvas;
    [SerializeField]
    TMP_Text tutorialText;
    int currTutorial = 0;
    [SerializeField]
    TutorialObject[] tutorials;
    //string[] tutorials = new string[7];
    public void NextTutorial()
    {
        currTutorial++;
        if (currTutorial == tutorials.Length)
        {
            ResetTutorials();
            tutorialCanvas.SetActive(false);
            return;
        }
        if (tutorials[currTutorial - 1].hintObject != null)
            tutorials[currTutorial - 1].hintObject.SetActive(false);
        if(tutorials[currTutorial].hintObject != null)
            tutorials[currTutorial].hintObject.SetActive(true);
        tutorialText.text = tutorials[currTutorial].hintText;
    }
    void ResetTutorials()
    {
        currTutorial = 0;
        for (int i = 1; i < tutorials.Length; i++)
        {
            if(tutorials[i].hintObject != null)
                tutorials[i].hintObject.SetActive(false);
        }
        if (tutorials[0].hintObject != null)
            tutorials[0].hintObject.SetActive(true);

        tutorialText.text = tutorials[currTutorial].hintText;
    }
}
