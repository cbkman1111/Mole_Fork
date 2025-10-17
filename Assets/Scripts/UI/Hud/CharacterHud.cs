using UnityEngine;
using UnityEngine.UI;

public class CharacterHud : MonoBehaviour
{
    [SerializeField] private Image Balloon = null; 
    [SerializeField] private TMPro.TextMeshProUGUI Msg = null;

    public bool Init()
    {
        Msg.SetActive(false);
        Balloon.SetActive(false);

        return true;
    }

    public void SetMessage(string msg)
    {
        if (Balloon == null || Msg == null)
            return;

        Msg.text = msg;
        Msg.SetActive(msg != string.Empty);
        Balloon.SetActive(msg != string.Empty);
    }
}
