using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Task_UI : MonoBehaviour
{
    public TMP_Text content;
    public TMP_Text progress;

    public void SetProperties(string content, string progress)
    {
        this.content.text = content;
        if(progress == null || progress == "0") { this.progress.text = ""; }
        else { this.progress.text = progress; }    
    }
}
