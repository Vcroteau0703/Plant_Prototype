using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldLeafUI : MonoBehaviour
{
    public int maxColl;
    public int currentColl;

    private TextMeshProUGUI currTxt;
    private TextMeshProUGUI maxTxt;
    private Player player;

    private void Awake()
    {
        currTxt = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        maxTxt = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        maxTxt.text = "/ " + maxColl.ToString();
        currTxt.text = currentColl.ToString();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        UpdateLeafUI();
    }

    public void UpdateLeafUI()
    {
        if (player.currColl != currentColl)
        {
            currentColl = player.currColl;
        }
        currTxt.text = currentColl.ToString();
    }

}
