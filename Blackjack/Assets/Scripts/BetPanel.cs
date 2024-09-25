using UnityEngine;

public class BetPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveBetPanelOffscreen()
    {
        transform.Translate(0, -20, 0);
    }

    public void MoveBetPanelOnscreen()
    {
        transform.Translate(0, 20, 0);
    }
}
