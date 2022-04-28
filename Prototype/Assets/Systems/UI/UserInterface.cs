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
    public static UserInterface instance;

    public Canvas canvas;

    public GameObject pauseMenu;

    private Controls inputs;

    private void OnEnable()
    {
        instance = this;
    }

    private void Awake()
    {
        if (inputs == null)
        {
            inputs = new Controls();
        }

        inputs.Player.Pause.performed += Pause;
        inputs.Player.Pause.Enable();

        if(canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        if (inputs != null)
        {
            inputs.Player.Pause.Disable();
        }
    }
}
