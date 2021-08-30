using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    public static UIPopup singleton;
    public GameObject blockRaycastPanel;
    public GameObject panel;
    public Text messageText;

    public void Start()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    public void Show(string message)
    {
        messageText.text = message;

        panel.SetActive(true);
        Debug.Log(message);
        if (blockRaycastPanel != null) blockRaycastPanel.SetActive(true);
    }

}
