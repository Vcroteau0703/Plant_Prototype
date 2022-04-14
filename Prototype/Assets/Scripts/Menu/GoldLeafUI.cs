using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldLeafUI : MonoBehaviour
{
    public int maxLeaves;

    public TextMeshProUGUI currTxt;
    public TextMeshProUGUI maxTxt;

    private void OnEnable()
    {
        maxTxt.text = "/ " + maxLeaves.ToString();
        currTxt.text = Player.goldenLeaves.Length.ToString();
    }
}
