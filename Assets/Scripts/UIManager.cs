using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI validityText;

    public TMP_Dropdown buildOptionDropdown;
    private Action<string> builderSelectionCallback;

    readonly Dictionary<int, string> buildOptions = new Dictionary<int, string>();
    private void Start()
    {
        buildOptionDropdown.onValueChanged.AddListener(BuildOptionSelected);
    }

    public void UpdateValidityText(bool isValid, string reason = "")
    {
        validityText.gameObject.SetActive(!isValid);
    }

    public void SetBuilderUI(bool enabled)
    {
        return; // TODO: Implement this
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
