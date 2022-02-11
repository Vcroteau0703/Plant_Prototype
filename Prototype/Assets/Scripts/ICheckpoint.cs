using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheckpoint
{
    public void Update_Checkpoint(Checkpoint checkpoint);
    public Checkpoint Get_Active_Checkpoint();
}
