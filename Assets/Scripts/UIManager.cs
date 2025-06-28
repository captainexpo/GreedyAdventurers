using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI validityText;

    public List<TextMeshProUGUI> phaseTexts;

    public void UpdateValidityText(bool isValid)
    {
        validityText.gameObject.SetActive(!isValid);
    }

    public void UpdatePhaseText(string phaseName)
    {
        foreach (var text in phaseTexts)
        {
            text.gameObject.SetActive(text.text == phaseName);
        }
    }

    public void SetBuilderUI(bool enabled)
    {
        return; // TODO: Implement this
    }
}
