using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI validityText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI roundText;

    public TMP_Dropdown buildOptionDropdown;
    private Action<string> builderSelectionCallback;

    public List<GameObject> builderExclusiveUI = new();
    public List<GameObject> mainMenuUI = new();

    readonly Dictionary<int, string> buildOptions = new();
    private void Start()
    {
        buildOptionDropdown.onValueChanged.AddListener(BuildOptionSelected);
    }

    public void UpdateValidityText((bool, string) x)
    {
        var (isValid, text) = x;
        validityText.gameObject.SetActive(!isValid);
        validityText.text = "Invalid Dungeon: " + text;
    }
    public void UpdateMoneyText(int money)
    {
        if (moneyText == null) Debug.LogWarning("What the fuck");
        moneyText.text = $"{money}$";
    }
    public void SetBuilderUI(bool visible)
    {
        foreach (var ui in builderExclusiveUI)
        {
     
            ui.SetActive(visible);
        }
    }
    public void SetMenuUI(bool visible)
    {
        print("Setting menu UI to " + visible);
        foreach (var ui in mainMenuUI)
        {
            ui.SetActive(visible);
        }
    }
    public void UpdateRoundText(int round)
    {
        roundText.text = "Round " + round;
    }
    public void SetBuildOptionDropdownSelectionCallback(Action<string> callback)
    {
        builderSelectionCallback = callback;
    }
    public void AddBuildOptionDropdown(string title, string text)
    {
        buildOptions.Add(buildOptionDropdown.options.Count, title);
        buildOptionDropdown.AddOptions(new List<string> { text } );
    }
    public void BuildOptionSelected(int index)
    {
        if (buildOptions.TryGetValue(index, out string title))
        {
            builderSelectionCallback?.Invoke(title);
        }
        else
        {
            Debug.LogWarning($"Build option with index {index} not found.");
        }
    }
}
