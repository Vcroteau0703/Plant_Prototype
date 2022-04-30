using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class LoadMenu : MonoBehaviour
{
    public GameObject saveButton_prefab;
    public RectTransform savesContent;

    private Controls input;

    private void OnEnable()
    {
        input ??= new Controls();
        input.UI.Primary_Action.performed += Initiate_File_Delete;
        input.UI.Primary_Action.Enable();
        LoadSaves(SaveSystem.GetSaveNames());
    }
    private void OnDisable()
    {
        input.UI.Primary_Action.performed -= Initiate_File_Delete;
        input.UI.Primary_Action.Disable();
    }

    public void Initiate_File_Delete(InputAction.CallbackContext ctx)
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if(selected.TryGetComponent(out FileLoad f))
        {
            ActionWindow.ButtonFunction function = f.Delete;
            Notification_System.Send_ActionWindow("Do you want to delete " + f.fileName.text + "?", "Delete", function);
        }  
    }

    public void LoadSaves(string[] fileNames)
    {
        savesContent.sizeDelta = new Vector2(savesContent.sizeDelta.x, 125f * fileNames.Length);

        for (int i = 0; i < savesContent.childCount; i++)
        {
            Destroy(savesContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < fileNames.Length; i++)
        {
            GameObject GO = Instantiate(saveButton_prefab, savesContent);
            GO.name = fileNames[i];
            GO.GetComponentInChildren<TMP_Text>().text = fileNames[i];
        }
    }

    private void Update()
    {
        if((savesContent.sizeDelta.y / 125f) != savesContent.childCount)
        {
            int diff = savesContent.childCount - ((int)savesContent.sizeDelta.y / 125);
            savesContent.sizeDelta = new Vector2(savesContent.sizeDelta.x, savesContent.sizeDelta.y + (125 * diff));
        }
    }
}
