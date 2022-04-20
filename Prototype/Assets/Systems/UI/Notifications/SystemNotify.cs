using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemNotify : MonoBehaviour
{
    private Transform parent;
    public TMP_Text message;
    private float lifeTime;
    private bool active;

    public void SetAttributes(string message)
    {
        this.message.text = message;
    }
    public void SetAttributes(string message, Color color)
    {
        this.message.text = message;
        this.message.color = color - new Color(0,0,0,color.a);
    }

    private void OnEnable()
    {
        parent = gameObject.transform.parent;
        Transform last = parent.GetChild(parent.childCount-1);
        if (last == transform && parent.GetChild(0) != transform) { active = false; }
        else { active = true; }
        message.color = new Color(message.color.r, message.color.g, message.color.b, 0);       
        lifeTime = 0.10f * message.text.Length;
        if (lifeTime < 2) { lifeTime = 2; }
        StartCoroutine(Life());
    }

    private IEnumerator Life()
    {    
        yield return new WaitUntil(() => parent.GetChild(0) == transform);
        float alpha = message.color.a;      
        while(alpha < 1f)
        {
            alpha = Mathf.Clamp01(alpha + Time.deltaTime * 2);
            message.color = new Color(message.color.r, message.color.g, message.color.b, alpha);
            yield return new WaitForFixedUpdate();
        }
        if (active){yield return new WaitForSeconds(lifeTime);}
        while (alpha > 0f)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime * 2);
            message.color = new Color(message.color.r, message.color.g, message.color.b, alpha);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
