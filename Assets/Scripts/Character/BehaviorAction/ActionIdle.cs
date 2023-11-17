using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIdle : Action
{
    protected TaskStatus OnUpdateIdle()
    {
        return TaskStatus.Failure;
    }
}
