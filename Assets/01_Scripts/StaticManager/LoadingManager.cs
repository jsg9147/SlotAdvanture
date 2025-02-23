using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static string nextScene;
    //[SerializeField] Image progressBar;
    [SerializeField] ProgressBarPro progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());

        InstantiateBackground();
    }

    void InstantiateBackground()
    {
        if (GameManager.Instance.stageData != null)
            Instantiate(GameManager.Instance.stageData.background);
        else
            Instantiate(MapGenerateData.Instance.tutorialStageBackground);
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LOADING");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.SetValue(Mathf.Lerp(progressBar.Value, op.progress, timer));
                if (progressBar.Value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.SetValue(Mathf.Lerp(progressBar.Value, 1f, timer));
                if (progressBar.Value >= 0.9999f && timer > 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
