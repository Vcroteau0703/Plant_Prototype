using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Control Settings", menuName = "Player Control Settings")]
public class Player_Control_Settings : ScriptableObject
{
    #region MOVEMENT VARIABLES
    [Header("Movement Settings")]
    public float move_speed = 1.0f;
    public float acceleration = 2.0f;
    public float friction = 0.4f;
    public LayerMask walkable;
    [Header("Jump Settings")]
    public Jump Jump = new Jump();
    public Jump Wall_Jump = new Jump();
    public float coyote_jump_delay = 0.15f;
    [Header("Air Control Settings")]
    public float Air_Speed = 1.0f;
    public float Air_Control = 1.0f;
    public float Air_Accel = 1.0f;
    public float Fall_Speed = 1.0f;
    [Header("Wall Slide Settings")]
    public float landing_slide_duration = 2.0f;
    public float landing_slide_speed = 5.0f;
    public float wall_slide_speed = 5.0f;
    public float wall_slide_smoothing = 0.1f;
    public float cling_strength = 1.0f;
    public float cling_release_power = 1.0f;
    [Header("Detection Settings")]
    public float slope_angle = 30f;
    public float ceiling_angle = 30f;
    public float cling_angle = 30f;
    public float min_climb_height = 1f;
    [Header("Input Settings")]
    [Range(0f, 100f)] public float horizontal_deadzone = 20f;
    [Range(0f, 100f)] public float vertical_deadzone = 20f;
    #endregion
}
