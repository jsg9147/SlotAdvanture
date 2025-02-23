using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;
using TMPro;

public class GameSetting : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider bgmVolumeSlider;

    [SerializeField] ResolutionDropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown languageDropdown;
    [SerializeField] Button closeBtn;
    [SerializeField] Toggle infoPopupOn;
    void Start()
    {
        LoadVolumeSetting();

        languageDropdown.onValueChanged.AddListener(SetLanguage);
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        SetLanguageDropdownValue();
        gameObject.SetActive(false);

        infoPopupOn.onValueChanged.AddListener(delegate
        {
            SetInfoPopupOnOff(infoPopupOn.isOn);
        });
    }

    void OnVolumeChanged(float volume)
    {
        MasterAudio.MasterVolumeLevel = volume;

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    void OnBgmVolumeChanged(float volume)
    {
        MasterAudio.PlaylistMasterVolume = volume;
        PlayerPrefs.SetFloat("BgmVolume", volume);
        PlayerPrefs.Save();
    }

    void LoadVolumeSetting()
    {
        // 이전에 저장한 볼륨 값 불러오기 (예를 들어, 플레이어의 설정에서 저장한 값)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float savedBgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1f);

        // Slider의 값 설정
        volumeSlider.value = savedVolume;
        bgmVolumeSlider.value = savedBgmVolume;

        // Slider의 OnValueChanged 이벤트에 함수 연결
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);

        // 이전에 저장한 볼륨 값을 Master Audio에 적용
        MasterAudio.MasterVolumeLevel = savedVolume;
        MasterAudio.PlaylistMasterVolume = savedBgmVolume;
    }

    void SetLanguage(int index)
    {
        string languageStr = languageDropdown.options[index].text;
        switch (languageStr)
        {
            case "Korea":
                PlayerPrefs.SetString("localizeKey", "ko");
                break;
            case "English":
                PlayerPrefs.SetString("localizeKey", "en");
                break;
            case "Chinese":
                PlayerPrefs.SetString("localizeKey", "cn");
                break;
        }
        GameManager.Instance.TextUpdate();
    }

    void SetLanguageDropdownValue()
    {
        string languageStr = PlayerPrefs.GetString("localizeKey", "en");

        switch(languageStr)
        {
            case "ko":
                languageDropdown.value = 0;
                break;
            case "en":
                languageDropdown.value = 1;
                break;
            case "cn":
                languageDropdown.value = 2;
                break;
        }
    }

    void SetInfoPopupOnOff(bool isOn)
    {
        GameManager.Instance.infoPopupOn = isOn;
        int on = isOn ? 0 : 1;

        PlayerPrefs.SetInt("PopupOn", on);
    }
}
