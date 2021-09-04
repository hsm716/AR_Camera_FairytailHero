using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IndexSample : MonoBehaviour
{
    public string nextScene;
    
    public GameObject warning;
    public Slider slider;
    private void Start()
    {
        warning.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.95f, Screen.height * 0.55f);
        warning.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.75f, Screen.height * 0.07f);
        slider.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Screen.height*0.1f);
        StartCoroutine(LoadAsynSceneCoroutine());
    }
    IEnumerator LoadAsynSceneCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        while (!operation.isDone)
        {
            slider.value = operation.progress;
            yield return null;
        }
    }
}