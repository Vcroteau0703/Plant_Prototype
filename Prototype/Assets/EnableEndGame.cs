using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableEndGame : MonoBehaviour
{
    public GameObject[] shitToShutOff;

    private void Start()
    {
        if(Quest_System.Get_Active_Event().name == "Return to the Fields"){
            transform.GetChild(0).gameObject.SetActive(true);
            foreach(GameObject shit in shitToShutOff)
            {
                shit.SetActive(false);
            }
        }
    }

    public void StartCredits()
    {
        SceneManager.LoadScene(4);
    }
}
