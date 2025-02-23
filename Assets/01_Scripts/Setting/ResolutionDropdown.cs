using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionDropdown : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    void Start()
    {
        SetDropdown();
    }

    void SetDropdown()
    {
        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            if (Is16by9Resolution(resolution))
            {
                //string optionText = ($"{resolution.width} x {resolution.height} @ {resolution.refreshRate}Hz");
                if (resolution.refreshRate == 60)
                {
                    string optionText = ($"{resolution.width} x {resolution.height}");
                    resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
                }
            }
        }

        SetDefaultDropdownValue();
        resolutionDropdown.onValueChanged.AddListener(DropdownValueChanged);
    }
    void SetDefaultDropdownValue()
    {
        string saveResolution = PlayerPrefs.GetString("resolution", $"{1600} x {900}");
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            resolutionDropdown.value = resolutionDropdown.options.Count-1;
            if (resolutionDropdown.options[i].text == saveResolution)
            {
                resolutionDropdown.value = i;
                break;
            }
        }
    }
    bool Is16by9Resolution(Resolution resolution)
    {
        float aspectRatio = (float)resolution.width / resolution.height;
        return Mathf.Approximately(aspectRatio, 16f / 9f);
    }
    void DropdownValueChanged(int index)
    {
        string selectedResolution = resolutionDropdown.options[index].text;
        SetResolution(selectedResolution);
    }
    void SetResolution(string resolution)
    {
        string[] parts = resolution.Split('x');
        if (parts.Length == 2)
        {
            int width = int.Parse(parts[0]);
            int height = int.Parse(parts[1]);
            Screen.SetResolution(width, height, Screen.fullScreen);
            PlayerPrefs.SetString("resolution", $"{width} x {height}");
            print($"ÇØ»óµµ : {width} x {height}");
        }
    }
}
