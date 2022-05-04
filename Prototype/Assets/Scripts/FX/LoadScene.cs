using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadScene : MonoBehaviour
{
    public string nextScene;

    private Controls inputs;

    //Mouse Stuff
    float timeLeft;
    float visibleCursorTimer = 2f;
    float cursorPosition;
    bool catchCursor = true;

    private void Awake()
    {
        if (inputs == null)
        {
            inputs = new Controls();
        }
        inputs.Player.Pointer.performed += EnableMouse;
        inputs.Player.Pointer.Enable();

    }

    public void LoadNextScene()
    {
        Laucher.LoadScene(nextScene);
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
}
