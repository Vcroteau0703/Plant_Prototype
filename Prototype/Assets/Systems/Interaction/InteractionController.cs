using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Interactions/InteractionController")]
public class InteractionController : MonoBehaviour
{
    Controls inputs;

    public Player player;

    #region INTERACTIONS
    public GameObject closestItem;
    [Header("Interaction Settings")]
    [Range(0, 10)]
    public float interactionRange;
    public bool showRange = false;
    #endregion
    
    private GameObject interactPrompt;

    private void OnEnable()
    {
        if (inputs == null)
        {
            inputs = new Controls();
        }
        inputs.Player.Interact.performed += Interact;
        inputs.Player.Enable();
    }

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        ScanInteractArea();
    }

    #region Inputs
    public void Interact(InputAction.CallbackContext context) // if user clicks pickup button
    {
        if (closestItem != null) // do we have an item to interact with
        {
            InteractionID id = closestItem.GetComponent<InteractionID>(); // get the interaction ID

            if (id.interactType == InteractionID.InteractType.stationary) //check the interaction type to prevent pickup of static objects Ex.(door or light switch)
            {
                id.GetComponent<Interact>().TriggerEvent(); // trigger the event on the item
                return;
            }
        }
    }
    #endregion

    #region Functions

    public void Setup()
    {
        InteractionData data = Resources.Load<InteractionData>("Data/Interaction Data");
        GameObject canvas = GameObject.Find("Interactions");
        if (!canvas)
        {          
            canvas = Instantiate(data.defaultTextCanvas, Vector3.zero, Quaternion.identity);
            canvas.name = "Interactions";
            interactPrompt = Instantiate(data.defaultInteractionText, canvas.transform);
            interactPrompt.SetActive(false);
            return;
        }
        interactPrompt = Instantiate(data.defaultInteractionText, canvas.transform);
        interactPrompt.SetActive(false);
    }
    public void ScanInteractArea() // searching for items to interact with
    {
        Collider[] objects = Physics.OverlapSphere(player.gameObject.transform.position, interactionRange);
        if (objects.Length > 0)
        {
            List<GameObject> interactables = new List<GameObject>(); // objects that the system found that have ID's

            for (int i = 0; i < objects.Length; i++) //filter out the objects that contain an interaction ID
            {
                if (objects[i].GetComponent<InteractionID>()) // does it have ID
                {
                    interactables.Add(objects[i].gameObject); // if it does add to list of interactable items
                }
            }
            if (interactables.Count > 0) // if the system found any interactable items
            {
                closestItem = GetClosestItem(player.gameObject.transform.position, interactables); // find the closest item to the player
                InteractionID id = closestItem.GetComponent<InteractionID>(); //get the ID
                DisplayInteractText(id.textPosition, id.InteractText); //display the interaction prompt on that item
            }
            else { closestItem = null; HideText(); } // if the system found no interactable items disable text
        }
    }
    public GameObject GetClosestItem(Vector2 playerPos, List<GameObject> items) // iterates through our interactable items that we found in ScanInteractArea() and returns the closest one
    {
        GameObject closestItem = items[0].gameObject;
        float minDistance = Vector2.Distance(playerPos, items[0].transform.position);
        for (int i = 0; i < items.Count; i++)
        {
            if (minDistance > Vector2.Distance(playerPos, items[i].transform.position))
            {
                minDistance = Vector2.Distance(playerPos, items[i].transform.position);
                closestItem = items[i];
            }
        }
        return closestItem;
    }

    #region UI Functions
    public void DisplayInteractText(Vector3 textPos, string text)
    {  
        interactPrompt.SetActive(true);
        InputDevice[] devices = InputSystem.devices.ToArray();
        JoystickData data = JoystickData.Get();

        foreach(InputDevice a in devices)
        {
            switch (a.displayName)
            {
                case "Xbox Controller":
                    Joystick_UIElements elements = data.GetJoystickUI("Xbox");
                    interactPrompt.GetComponent<InteractPrompt>().SetAttributes(elements.interact, text);
                    interactPrompt.transform.position = textPos;
                    return;

                default:
                    break;
            }
        }
        interactPrompt.GetComponent<InteractPrompt>().SetAttributes(null, "[" + inputs.Player.Interact.GetBindingDisplayString(0) + "]" + text);
        interactPrompt.transform.position = textPos;
    }
    public void HideText()
    {
        interactPrompt.SetActive(false);
    }
    #endregion
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        //displays the interact radius
        if (showRange && player.gameObject)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.gameObject.transform.position, interactionRange);
        }
    }
    #endregion
}
