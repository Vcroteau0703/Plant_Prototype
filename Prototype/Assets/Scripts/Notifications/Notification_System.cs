using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Notification_System
{
    private static Transform Notifications;
    private static NotificationData data;
    public static void Send_ActionWindow(string message, string buttonName, ActionWindow.ButtonFunction function)
    {
        RunSetup();
        NotificationData data = Resources.Load<NotificationData>("Data/Notification Data");
        GameObject newGO = Object.Instantiate(data.defaultActionWindow, Notifications);
        newGO.GetComponent<ActionWindow>().SetAttributes(message, buttonName, function);
    }

    public static void Send_SystemNotify(string message)
    {
        RunSetup();
        GameObject defaultOB = Resources.Load<NotificationData>("Data/Notification Data").defaultSystemNotify;
        GameObject newGO = Object.Instantiate(defaultOB, Notifications);
        newGO.GetComponent<SystemNotify>().SetAttributes(message);
    }

    public static void Send_SystemNotify(string message, Color color)
    {
        RunSetup();
        GameObject defaultOB = Resources.Load<NotificationData>("Data/Notification Data").defaultSystemNotify;
        GameObject newGO = Object.Instantiate(defaultOB, Notifications);
        newGO.GetComponent<SystemNotify>().SetAttributes(message, color);
    }

    public static void RunSetup()
    {
        data = Resources.Load<NotificationData>("Data/Notification Data");
        GameObject GO = GameObject.Find("Notifications");
        if (GO == null)
        {
            GO = Object.Instantiate(data.defaultNotificationCanvas);
            GO.GetComponent<Canvas>().worldCamera = Camera.main;
            GO.name = "Notifications";
            Notifications = GO.transform;
            return;
        }
        Notifications = GO.transform;
    }
}

