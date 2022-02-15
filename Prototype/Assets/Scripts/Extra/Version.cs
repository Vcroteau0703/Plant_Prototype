using UnityEngine;
using TMPro;

public class Version : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        text.text = Application.version;
    }
}
