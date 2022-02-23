using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Notification Data", menuName = "Notification Data")]
public class NotificationData : ScriptableObject
{
    public GameObject defaultNotificationCanvas;
    public GameObject defaultActionWindow;
    public GameObject defaultSystemNotify;
}
