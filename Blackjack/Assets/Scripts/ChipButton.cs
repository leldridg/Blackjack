using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipButton : MonoBehaviour
{
    public TMP_Text bankTxt;
    private TMP_Text chipValue;

    // Start is called before the first frame update
    void Start()
    {
        chipValue = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int chipValueInt = int.Parse(chipValue.text.Substring(1));
        int bankValueInt = int.Parse(bankTxt.text.Substring(1));
        if (chipValueInt > bankValueInt)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
}
