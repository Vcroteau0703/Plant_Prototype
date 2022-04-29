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
    private EventSystem sys;

    private void Awake()
    {
        sys = EventSystem.current;
    }

    public void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        if (First_Selected != null)
        {
            First_Selected.GetComponent<Button>().Select();
        }
    }

    public void OnDisable()
    {
        sys.SetSelectedGameObject(null);
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
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
