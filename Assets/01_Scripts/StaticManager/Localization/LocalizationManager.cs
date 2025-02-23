using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager _instance;

    public TMP_FontAsset nomalFont;
    public TMP_FontAsset cnFont;

    // CSV 파일 경로
    public TextAsset uiLocalizationCsv;
    public TextAsset itemLocalizationCsv;
    public TextAsset skillLocalizationCsv;
    public TextAsset unitNameLocalizationCsv;
    public TextAsset monsterNameLocalizationCsv;

    // 언어별 텍스트를 저장할 딕셔너리
    private Dictionary<string, Dictionary<string, string>> uiLocalizationData;
    private Dictionary<string, Dictionary<string, string>> itemLocalizationData;
    private Dictionary<string, Dictionary<string, string>> skillLocalizationData;
    private Dictionary<string, Dictionary<string, string>> unitNameLocalizationData;
    private Dictionary<string, Dictionary<string, string>> monsterNameLocalizationData;

    public static LocalizationManager Instance
    {
        get { return _instance; }
    }
    void Awake()
    {
        _instance = this;

        LoadLocalizationData();
        LoadItemLocalizationData();
        LoadSkillLocalizationData();
        LoadUnitNameLocalizationData();
        LoadMonsterNameLocalizationData();
        //DisplayText("en");  // 언어 선택 (예: 영어)
    }

    void LoadLocalizationData()
    {
        try
        {
            uiLocalizationData = new Dictionary<string, Dictionary<string, string>>();

            // CSV 파일을 행별로 분할
            string[] lines = uiLocalizationCsv.text.Split('\n');

            // 첫 번째 행은 언어 코드
            string[] languages = lines[0].Trim().Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Trim().Split(',');

                string key = values[0];
                Dictionary<string, string> languageTexts = new Dictionary<string, string>();

                for (int j = 1; j < values.Length; j++)
                {
                    languageTexts.Add(languages[j].Trim(), values[j].Trim());
                }

                uiLocalizationData.Add(key, languageTexts);
            }
        }
        catch (System.IndexOutOfRangeException ex)
        {
            print(ex);
        }
    }

    void LoadItemLocalizationData()
    {
        itemLocalizationData = new Dictionary<string, Dictionary<string, string>>();

        // CSV 파일을 행별로 분할
        string[] lines = itemLocalizationCsv.text.Split('\n');

        // 첫 번째 행은 언어 코드
        string[] languages = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');

            string key = values[0];
            Dictionary<string, string> languageTexts = new Dictionary<string, string>();

            for (int j = 1; j < values.Length; j++)
            {
                languageTexts.Add(languages[j].Trim(), values[j].Trim());
            }

            itemLocalizationData.Add(key, languageTexts);
        }
    }
    void LoadSkillLocalizationData()
    {
        skillLocalizationData = new Dictionary<string, Dictionary<string, string>>();

        // CSV 파일을 행별로 분할
        string[] lines = skillLocalizationCsv.text.Split('\n');

        // 첫 번째 행은 언어 코드
        string[] languages = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');

            string key = values[0];
            Dictionary<string, string> languageTexts = new Dictionary<string, string>();

            for (int j = 1; j < values.Length; j++)
            {
                languageTexts.Add(languages[j].Trim(), values[j].Trim());
            }

            skillLocalizationData.Add(key, languageTexts);
        }
    }

    void LoadUnitNameLocalizationData()
    {
        unitNameLocalizationData = new Dictionary<string, Dictionary<string, string>>();

        // CSV 파일을 행별로 분할
        string[] lines = unitNameLocalizationCsv.text.Split('\n');

        // 첫 번째 행은 언어 코드
        string[] languages = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');

            string key = values[0];
            Dictionary<string, string> languageTexts = new Dictionary<string, string>();

            for (int j = 1; j < values.Length; j++)
            {
                languageTexts.Add(languages[j].Trim(), values[j].Trim());
            }

            unitNameLocalizationData.Add(key, languageTexts);
        }
    }

    void LoadMonsterNameLocalizationData()
    {
        monsterNameLocalizationData = new Dictionary<string, Dictionary<string, string>>();

        // CSV 파일을 행별로 분할
        string[] lines = monsterNameLocalizationCsv.text.Split('\n');

        // 첫 번째 행은 언어 코드
        string[] languages = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');

            string key = values[0];
            Dictionary<string, string> languageTexts = new Dictionary<string, string>();

            for (int j = 1; j < values.Length; j++)
            {
                languageTexts.Add(languages[j].Trim(), values[j].Trim());
            }

            monsterNameLocalizationData.Add(key, languageTexts);
        }
    }


    public string GetUILocalizingText(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");

        if (uiLocalizationData.ContainsKey(key))
        {
            return uiLocalizationData[key][languageKey];
        }
        else
        {
            return "";
        }
    }

    public string GetItemLocalizingName(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");

        try
        {
            if (itemLocalizationData.ContainsKey(key))
            {
                return itemLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetItemLocalizingDescription(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        languageKey = languageKey + "Script";
        try
        {
            if (itemLocalizationData.ContainsKey(key))
            {
                return itemLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetSkillLocalizingName(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        try
        {
            if (skillLocalizationData.ContainsKey(key))
            {
                return skillLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetSkillLocalizingDescription(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        languageKey = languageKey + "Script";
        try
        {
            if (skillLocalizationData.ContainsKey(key))
            {
                return skillLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetSkillBookLocalizingDescription(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        languageKey = languageKey + "BookScript";
        try
        {
            if (skillLocalizationData.ContainsKey(key))
            {
                return skillLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetSkillLocalizingBuffDescription(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        languageKey = languageKey + "BuffScript";
        try
        {
            if (skillLocalizationData.ContainsKey(key))
            {
                return skillLocalizationData[key][languageKey];
            }
            else
            {
                print($"{languageKey} : {key}");
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetUnitLocalizingName(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        try
        {
            if (unitNameLocalizationData.ContainsKey(key))
            {
                return unitNameLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
    }

    public string GetMonsterLocalizingName(string key)
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        try
        {
            if (monsterNameLocalizationData.ContainsKey(key))
            {
                return monsterNameLocalizationData[key][languageKey];
            }
            else
            {
                return "";
            }
        }
        catch (KeyNotFoundException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "키가 없다";
        }
        catch (System.ArgumentNullException ex)
        {
            print(ex);
            print($"{languageKey} : {key}");
            return "코드 값";
        }
    }

    public TMP_FontAsset GetFont()
    {
        string languageKey = PlayerPrefs.GetString("localizeKey", "en");
        switch (languageKey)
        {
            case "en":
            case "ko":
                return nomalFont;
            case "cn":
                return cnFont;
            default:
                return nomalFont;
        }
    }
}
