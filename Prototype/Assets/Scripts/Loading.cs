using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Loading : MonoBehaviour
{
    public TMP_Text progressText;
    private AsyncOperation target;
    void Start()
    {
        int scene = SaveSystem.Load<Portal_Data>("/Temp/Portal.data").nextScene;
        target = SceneManager.LoadSceneAsync(scene);
        StartCoroutine(Activate_Scene());
    }

    IEnumerator Activate_Scene()
    {  
        progressText.text = "0%";
        target.allowSceneActivation = false;
        while (progressText.text != "100%")
        {
            float current = float.Parse(progressText.text.Replace("%", ""));
            if(current < target.progress/0.9f * 100)
            {
                progressText.text = (current + 1f).ToString() + "%";
            }
            else
            {
                progressText.text = (target.progress / 0.9f * 100).ToString() + "%";
            }          
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1.5f);
        target.allowSceneActivation = true;
    }
}
