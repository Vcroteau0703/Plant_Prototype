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

    //Mouse Stuff
    float timeLeft;
    float visibleCursorTimer = 2f;
    float cursorPosition;
    bool catchCursor = true;

    private void OnEnable()
    {
        instance = this;
    }

    public void DisablePause()
    {
        inputs.Player.Pause.Disable();
    }

    public void EnablePause()
    {
        inputs.Player.Pause.Enable();
    }

    private void Awake()
    {
        if (inputs == null)
        {
            inputs = new Controls();
        }

        inputs.Player.Pause.performed += Pause;
        inputs.Player.Pause.Enable();
        inputs.Player.Pointer.performed += EnableMouse;
        inputs.Player.Pointer.Enable();

        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    void EnableMouse(InputAction.CallbackContext context)
    {
        timeLeft = visibleCursorTimer;
        Cursor.visible = true;
        catchCursor = false;
    }

    void EnableMouse()
    {
        timeLeft = visibleCursorTimer;
        Cursor.visible = true;
        catchCursor = false;
    }

    void Update()
    {
        if (!catchCursor)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timeLeft = visibleCursorTimer;
                Cursor.visible = false;
                catchCursor = true;
            }
        }
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

    public void Pause()
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
        if (inputs != null)
        {
            inputs.Player.Pause.Disable();
        }
    }
}
