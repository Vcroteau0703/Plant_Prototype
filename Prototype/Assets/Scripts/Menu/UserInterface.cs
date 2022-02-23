using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This script will be used for all UI implementation
 * 
 */

public class UserInterface : MonoBehaviour
{
    public Canvas canvas;

    public GameObject pauseMenu;

    private Controls inputs;

    private void Awake()
    {
        if (inputs == null)
        {
            inputs = new Controls();
        }

        inputs.Player.Pause.performed += Pause;
        inputs.Player.Pause.Enable();

        canvas.worldCamera = Camera.main;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            return;
        }
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
        inputs.Player.Pause.Disable(); 
    }
}
