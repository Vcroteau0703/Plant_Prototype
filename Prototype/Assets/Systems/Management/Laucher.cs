using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Laucher
{  
    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
        AsyncOperation levelLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        levelLoad.completed += LevelLoad_completed;
    }

    private static void LevelLoad_completed(AsyncOperation obj)
    {
        obj.allowSceneActivation = true;
    }

    public static IEnumerator LoadScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void CloseApplication()
    {
        Application.Quit();
    }

    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
