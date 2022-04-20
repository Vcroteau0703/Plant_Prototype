using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.Playables;
using Yarn.Unity;

public class Debug_Console : MonoBehaviour
{
    private delegate void Method(object args);
    private Method action = (args) => { };
    private Controls controls;
    public TMPro.TMP_InputField input;
    private void OnEnable()
    {
        controls ??= new Controls();
        controls.Debug.Enable();
        controls.Debug.Console.performed += Toggle_Console;
    }

    private void Toggle_Console(InputAction.CallbackContext context)
    {
        GameObject child = transform.GetChild(0).gameObject;
        child.SetActive(!child.activeSelf);

        if (child.activeInHierarchy) { Player_Controller.instance.controls.Player.Disable(); }
        else { Player_Controller.instance.controls.Player.Enable(); }

        if (EventSystem.current.currentSelectedGameObject) { EventSystem.current.SetSelectedGameObject(null); }
        EventSystem.current.SetSelectedGameObject(child);
        input.text = "";
    }
    private void Toggle_Console()
    {
        GameObject child = transform.GetChild(0).gameObject;
        child.SetActive(!child.activeSelf);

        if (child.activeInHierarchy) { Player_Controller.instance.controls.Player.Disable(); }
        else { Player_Controller.instance.controls.Player.Enable(); }

        if (EventSystem.current.currentSelectedGameObject) { EventSystem.current.SetSelectedGameObject(null); }
        EventSystem.current.SetSelectedGameObject(child);
        input.text = "";
    }

    // FORMAT

    // -> /spawn odin 2
    public void Run_Command(string command)
    {
        if (command.EndsWith("\n") && command.StartsWith("/"))
        {
            string arguments = null;
            string c = command.Replace("/", "").Replace("\n", "");
            if (command.Contains(" ")){
                arguments = c.Replace(c.Split(' ')[0] + " ", "");
                c = c.Split(' ')[0];               
            }
            string first = c[0].ToString().ToUpper();
            c = first + c.Substring(1, c.Length - 1);
            
            Debug.Log("c is " + c + " length is " + c.Length);
            Debug.Log("Arguments: " + arguments);
            MethodInfo method_info = this.GetType().GetMethod(c,  BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Debug.Log(method_info);
            if (method_info != null){
                action = (Method)System.Delegate.CreateDelegate(typeof(Method), this, method_info);
                action(arguments);
            }
            else{
                Notification_System.Send_SystemNotify("Command not found", Color.red);
            }
            input.text = "";
            Toggle_Console();
        }        
        else if (command.EndsWith("\n")) { input.text = ""; }
    }

    DialogueRunner r;
    private void Update()
    {
        if (r != null)
        {
            Debug.Log(r.IsDialogueRunning);
        }
    }

    public void Kill(object args)
    {
        if (args != null)
        {
            Notification_System.Send_SystemNotify("Method requires 0 argument(s)", Color.red);
            return;
        }
        Player player = Player_Controller.instance.GetComponent<Player>();
        if(player != null){
            player.Respawn();
        }
        else {
            Notification_System.Send_SystemNotify("Could not find Player", Color.red);
        }
    }
    public void Quit(object args)
    {
        if(args != null)
        {
            Notification_System.Send_SystemNotify("Method requires 0 argument(s)", Color.red);
            return;
        }
        Application.Quit();
    }
    public void Pause(object args)
    {
        if (args != null)
        {
            Notification_System.Send_SystemNotify("Method requires 0 argument(s)", Color.red);
            return;
        }
        UserInterface.instance.Pause();
    }
    public void Save(object args)
    {
        if(args == null)
        {
            GameManager.SaveGame();
            return;
        }
        Notification_System.Send_SystemNotify("Method requires 0 argument(s)", Color.red);
    }
    public void Load(object args)
    {
        if(args == null){
            Notification_System.Send_SystemNotify("Method requires 1 argument(s)", Color.red);
            return;
        }

        int index = -1;
        string name = null;


        if (int.TryParse(args.ToString(), out index))
        {
            name = null;
        }
        else
        {
            name = args.ToString();
            index = -1;
        }

        if (SceneManager.sceneCountInBuildSettings > index && index != -1)
        {
            Debug.Log("Loading Level " + index);
            SceneManager.LoadScene(index);
            return;
        }
        else if (name != null)
        {
            Debug.Log("Trying to Load " + name);

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                Debug.Log("Scene " + i + " " + SceneUtility.GetScenePathByBuildIndex(i));
                if(SceneUtility.GetScenePathByBuildIndex(i).Contains(name))
                {
                    Debug.Log("Loading " + name);
                    SceneManager.LoadScene(name);
                    return;
                }
            }          
        }
        else
        {
            Notification_System.Send_SystemNotify("Scene does not exist in the build settings", Color.red);
        }
    }
    public void Skip(object args)
    {
        DialogueRunner d = FindObjectOfType<DialogueRunner>();
        if(d != null)
        {
            d.Dialogue.Stop();
            d.IsDialogueRunning = false;
            d.Clear();

            System.Delegate[] a = d.Dialogue.CommandHandler.GetInvocationList();
            Debug.Log(a[0].Method.Name);
        }
    }
    public void Ability(object args)
    {
        if (args == null)
        {
            Notification_System.Send_SystemNotify("Method requires 2 argument(s)", Color.red);
            return;
        }
        
        string[] input = args.ToString().Split(' '); // "Idle 0"

        string name = input[0];
        name = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length-1);
        int enabled = int.Parse(input[1]);

        Debug.Log(name);

        Player_Controller player = Player_Controller.instance;
        if (!player) 
        {
            Notification_System.Send_SystemNotify("Command requires 1 instance of <Player Controller> in the scene", Color.red);
            return;
        }

        if(enabled == 1)
        {
            player.state_controller.Enable_State(name);
        }
        else if (enabled == 0)
        {
            player.state_controller.Disable_State(name);
        }
        else
        {
            Notification_System.Send_SystemNotify("Argument 2 of Ability <string> <int> must be 0 or 1", Color.red);
        }
    }

    public void Tp(object args)
    {
        switch (args.ToString()) 
        {
            case "Start":
                break;

            case "End":
                break;

            case "Mid":
                break;

            case "Ran":
                break;         
        }
    }
    private void OnDisable()
    {
        controls.Debug.Disable();
    }

}
