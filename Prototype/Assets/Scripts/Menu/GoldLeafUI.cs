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
        Player_Data data = SaveSystem.Load<Player_Data>("/Player/Player.data");

        maxTxt.text = "/ " + maxLeaves.ToString();
        currTxt.text = data.goldLeaves.Length.ToString();
    }
}
