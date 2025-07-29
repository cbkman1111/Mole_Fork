using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Foreach String",
        story: "Foreach [OutString] in [StringList]",
        category: "Flow",
        id: "906fb4a95cdd94956939bfe00c74126f")]
    public partial class ForeachStringModifier : Modifier
    {
        [SerializeReference] public BlackboardVariable<string> OutString;
        [SerializeReference] public BlackboardVariable<List<string>> StringList;
        [CreateProperty] private int m_CompletedRuns;
        
        protected override Status OnStart()
        {
            m_CompletedRuns = 0;
            if (Child == null)
            {
                LogFailure("No child subgraph to run.");
                return Status.Success;
            }

            if (StringList.Value.Count == 0)
            {
                return Status.Success;
            }

            OutString.Value = StringList.Value[0];
            var status = StartNode(Child);
            if (status == Status.Failure || status == Status.Success)
            {
                return Status.Running;
            }

            return Status.Waiting;
        }

        protected override Status OnUpdate()
        {
            Status status = Child.CurrentStatus;
            if (status == Status.Failure || status == Status.Success)
            {
                if (StringList.Value.Count != 0 && ++m_CompletedRuns >= StringList.Value.Count)
                {
                    return status;
                }

                OutString.Value = StringList.Value[m_CompletedRuns];
                var newStatus = StartNode(Child);
                if (newStatus == Status.Failure || newStatus == Status.Success)
                {
                    return Status.Running;
                }
            }
            return Status.Waiting;
        }
    }
}