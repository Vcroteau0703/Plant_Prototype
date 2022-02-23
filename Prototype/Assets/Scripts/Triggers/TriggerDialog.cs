using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TriggerDialog : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public string dialogueToRun;

    private void OnTriggerEnter(Collider other)
    {
        dialogueRunner.StartDialogue(dialogueToRun);
        gameObject.SetActive(false);
    }
}
