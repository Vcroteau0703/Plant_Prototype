using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public TMP_InputField saveNameInputField;
    public GameObject First_Selected;

    private void OnEnable()
    {
        EventSystem sys = EventSystem.current;
        sys.SetSelectedGameObject(First_Selected, new BaseEventData(sys));
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
        Laucher.LoadScene("Chris_test");
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
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
