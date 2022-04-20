using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Control Settings", menuName = "Player Control Settings")]
public class Player_Control_Settings : ScriptableObject
{
    #region MOVEMENT VARIABLES
    [Header("Movement Settings")]
    public float Move_Speed = 1.0f;
    public float Acceleration = 2.0f;
    public float Friction = 0.4f;
    [Header("Jump Settings")]
    public Jump Jump = new Jump();
    public Jump Wall_Jump = new Jump();
    public float Coyote_Delay = 0.15f;
    [Header("Air Control Settings")]
    public float Air_Speed = 1.0f;
    public float Air_Control = 1.0f;
    public float Air_Accel = 1.0f;
    public float Fall_Speed = 1.0f;
    [Header("Glide Settings")]
    [Range(0, 1)] public float Glide_Power = 1.1f;
    [Header("Wall Slide Settings")]
    public float Slide_Friction = 1.0f;
    public float Max_Slide_Speed = 8.0f;
    public float Slide_Threshhold = 1.0f;
    public float Slide_Accel = 1.0f;
    public float Cling_Power = 1.0f;
    public float Eject_Power = 1.0f;
    [Header("Detection Settings")]
    public float Slope_Angle = 30f;
    public float Ceiling_Angle = 30f;
    public float Cling_Threshold = 1f;
    [Header("Ceiling Settings")]
    public float Bonk_Power = 2f;
    [Header("Input Settings")]
    [Range(0f, 100f)] public float horizontal_deadzone = 20f;
    [Range(0f, 100f)] public float vertical_deadzone = 20f;
    #endregion
}
