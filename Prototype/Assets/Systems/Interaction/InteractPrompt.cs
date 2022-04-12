using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InteractPrompt : MonoBehaviour
{
    public Image image;
    public TMP_Text text;

    public void SetAttributes(Sprite image, string text)
    {
        if(image == null) { this.image.gameObject.SetActive(false); }
        else { this.image.gameObject.SetActive(true); this.image.sprite = image; }
        
        this.text.text = text;
    }
}
