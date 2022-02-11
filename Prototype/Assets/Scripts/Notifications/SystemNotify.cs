using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemNotify : MonoBehaviour
{
    public TMP_Text message;
    float lifeTime;

    public void SetAttributes(string message)
    {
        this.message.text = message;
    }
    public void SetAttributes(string message, Color color)
    {
        this.message.text = message;
        this.message.color = color;
    }

    private void OnEnable()
    {
        Transform parent = gameObject.transform.parent;
        for(int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject != gameObject)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        lifeTime = 0.18f * message.text.Length;
        if (lifeTime < 2) { lifeTime = 2; }
    }

    private void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) { Destroy(gameObject); }
        message.color = message.color - new Color(0, 0, 0, message.color.a * (1f * Time.deltaTime));
    }
}
