using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Dialog_Volume : Action_Volume
{
    public DialogueRunner dialogueRunner;
    public string dialogueToRun;

    new private void Awake()
    {
        base.Awake();
        action += Start_Dialog;
    }

    private void Start_Dialog(GameObject actor)
    {
        dialogueRunner.StartDialogue(dialogueToRun);
    }
}
