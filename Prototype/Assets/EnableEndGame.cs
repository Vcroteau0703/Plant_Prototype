using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EnableEndGame : MonoBehaviour
{
    public GameObject taskList;
    public GameObject[] shitToShutOff;
    public GameObject[] shitToTurnOn;

    private void Start()
    {
        StartCoroutine(ActivateEnding());
    }

    IEnumerator ActivateEnding()
    {
        yield return new WaitForSeconds(2f);
        //Debug.Log(taskList.transform.GetChild(0).Find("Description").GetComponent<TextMeshProUGUI>().text);
        if(taskList.transform.GetChild(0) != null)
        {
            if (taskList.transform.GetChild(0).Find("Description").GetComponent<TextMeshProUGUI>().text == "Return to the Fields")
            {
                foreach (GameObject shit in shitToTurnOn)
                {
                    shit.SetActive(true);
                }
                foreach (GameObject shit in shitToShutOff)
                {
                    shit.SetActive(false);
                }
            }
        }

    }

    public void StartCredits()
    {
        SceneManager.LoadScene(4);
    }
}
