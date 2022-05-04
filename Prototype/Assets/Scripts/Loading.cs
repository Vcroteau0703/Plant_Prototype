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
        string scene = SaveSystem.Load<Player_Data>("/Player/Player.data").scene;
        target = SceneManager.LoadSceneAsync(scene);
        target.allowSceneActivation = true;
    }

    private void LateUpdate()
    {
        progressText.text = (target.progress * 100).ToString("00") + "%";
    }
}
