using UnityEngine;

[AddComponentMenu("Interactions/InteractionID")]
public class InteractionID : MonoBehaviour, ISerializationCallbackReceiver
{
    public enum InteractType { pickup, stationary }

    public InteractType interactType;

    public string InteractText;
    public Vector3 textOffset;
    [HideInInspector] public Vector3 textPosition = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(textPosition, transform.forward / 3 + transform.right + Vector3.up / 2);
    }

    private void Update()
    {
        textPosition = gameObject.transform.position + textOffset;
    }

    public void OnBeforeSerialize()
    {
        if(textPosition != gameObject.transform.position + textOffset)
        {
            textPosition = gameObject.transform.position + textOffset;
        }
    }

    public void OnAfterDeserialize()
    {
    }
}
