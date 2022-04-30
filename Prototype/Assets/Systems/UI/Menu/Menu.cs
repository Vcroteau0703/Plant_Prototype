using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public TMP_InputField saveNameInputField;

    IEnumerator Set_Selected(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        obj.GetComponent<Button>().Select();
    }

    public void EnableButton(Button button)
    {
        if (saveNameInputField.text.Length > 0)
        {
            button.interactable = true;
            return;
        }
        button.interactable = false;
    }

    public void CreateNewSave()
    {
        SaveSystem.CreateNewSave(saveNameInputField.text);
    }

    public void LoadScene(string sceneName)
    {
        Laucher.LoadScene(sceneName);
    }

    public void SaveGame()
    {
        GameManager.SaveGame();       
    }

    public void Resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void OpenMenu(GameObject menu)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.activeInHierarchy)
            {
                child.SetActive(false);
            }
        }
        menu.SetActive(true);

        Button[] buttons = menu.GetComponentsInChildren<Button>();

        foreach (Button b in buttons)
        {
            if(b.gameObject.CompareTag("First Selection"))
            {
                StartCoroutine(Set_Selected(b.gameObject));
            }
        }

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
