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
    public float jump_power = 1.0f;
    public float jump_height = 10.0f;
    public float jump_buffer = 0.2f;
    public float weight = 10.0f;
    public float coyote_jump_delay = 0.15f;
    [Header("Air Control Settings")]
    public float air_control = 1.0f;
    public float max_air_speed = 10.0f;
    public float max_fall_speed = 20.0f;
    [Header("Wall Jump Settings")]
    public float wall_jump_power = 10.0f;
    public float wall_grab_strength = 20.0f;
    public float wall_jump_buffer = 0.15f;
    public float wall_jump_angle = 45.0f;
    public float wall_release_strength = 5.0f;
    [Header("Wall Slide Settings")]
    public float landing_slide_duration = 2.0f;
    public float landing_slide_speed = 5.0f;
    public float wall_slide_speed = 5.0f;
    public float wall_slide_smoothing = 0.1f;
    [Header("Detection Settings")]
    public float slope_angle = 30f;
    public float ceiling_angle = 30f;
    public float min_climb_height = 1f;
    [Header("Input Settings")]
    [Range(0f, 100f)] public float horizontal_deadzone = 20f;
    [Range(0f, 100f)] public float vertical_deadzone = 20f;
    #endregion
}
