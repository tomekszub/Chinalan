using UnityEngine;
public class CheckBox : MonoBehaviour
{
    [SerializeField]
    GameObject checkBoxImg;
    public void ToggleCheckBox()
    {
        checkBoxImg.SetActive(!checkBoxImg.activeSelf);
    }
}