using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMask : MonoBehaviour
{
    public float ProgressBarSpeed = 10;
    public string LoadingSceneName { get; set; }
    public event UnityAction OnFadedIn = () => { };

    Slider progressBar;
    Text progressText;
    AsyncOperation operation;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        progressBar = transform.Find("Progress Bar").GetComponent<Slider>();
        progressText = transform.Find("Progress Text").GetComponent<Text>();
    }

    /// <summary>
    /// 这个函数会在淡入动画的最后调用
    /// </summary>
    public void OnOpenAnimationFinished()
    {
        OnFadedIn();
        StartCoroutine(AsyncLoad());
    }

    IEnumerator AsyncLoad()
    {
        yield return null;

        operation = SceneManager.LoadSceneAsync(LoadingSceneName);
        operation.allowSceneActivation = false;
        while (!operation.isDone) {
            var prog = operation.progress;
            if (prog >= 0.9f) {
                operation.allowSceneActivation = true;
            }
            progressBar.value = Mathf.Lerp(progressBar.value, prog, ProgressBarSpeed * Time.deltaTime);
            progressText.text = (int)(prog * 100) + "%";
            yield return null;
        }
        progressBar.value = Mathf.Lerp(progressBar.value, 1, ProgressBarSpeed * Time.deltaTime);
        progressText.text =  100 + "%";
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在淡出动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.gameObject);
    }
}
