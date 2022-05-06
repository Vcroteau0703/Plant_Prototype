using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade_Load : MonoBehaviour
{
    public void AfterFade()
    {
        SceneManager.LoadScene("Loading");
    }
}
