using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Image[] leaves;
    public Color[] healthColors;

    public void UpdateUI(int currHealth)
    {
        health = currHealth;
        for (int i = 0; i < leaves.Length; i++)
        {
            if(i < health)
            {
                leaves[i].color = healthColors[0];
            }
            else
            {
                leaves[i].color = healthColors[1];
            }
        }
    }

}
