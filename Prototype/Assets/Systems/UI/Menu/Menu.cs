using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public TMP_InputField saveNameInputField;

    private void OnEnable()
    {
        GameObject target = Get_First_Selection(gameObject);
        if(target != null){StartCoroutine(Set_Selected(target));}
    }

    IEnumerator Set_Selected(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(null);
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
        StartCoroutine(Set_Selected(Get_First_Selection(menu)));
    }

    public GameObject Get_First_Selection(GameObject menu)
    {
        Button[] buttons = menu.GetComponentsInChildren<Button>();

        foreach (Button b in buttons)
        {
            if (b.gameObject.CompareTag("First Selection"))
            {
                return b.gameObject;            
            }
        }
        return null;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
