using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI validityText;
    public void UpdateValidityText(bool isValid)
    {
        validityText.gameObject.SetActive(!isValid);
    }
}
