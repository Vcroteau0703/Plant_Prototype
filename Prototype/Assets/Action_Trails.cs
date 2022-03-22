using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class Action_Trails : MonoBehaviour
{
    public float threshhold;

    public ParticleSystem ps;
    public Rigidbody connected_rb;
    ParticleSystem.TrailModule trail;

    public float origin;

    private void Start()
    {
        trail = ps.trails;
        origin = trail.ratio;
    }
    private void Update()
    {
        trail.ratio = connected_rb.velocity.magnitude > threshhold ? 1 : 0;
    }
}
