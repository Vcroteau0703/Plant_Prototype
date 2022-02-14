using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadMenu : MonoBehaviour
{
    public GameObject saveButton_prefab;
    public RectTransform savesContent;
    private void OnEnable()
    {
        LoadSaves(SaveSystem.GetSaveNames());
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
