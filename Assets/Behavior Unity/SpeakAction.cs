using Creature;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Speak", story: "[Agent] Speak [Message]", category: "Action", id: "60b607631a69b5cc14db9463ed55ee82")]
public partial class SpeakAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<string> Message;

    private WorldObject agent = null;
    private BlackboardReference blackBoard = null;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        GameObject go = Agent.Value;
        if (go == null)
            return Status.Failure;

        agent = go.GetComponent<WorldObject>();
        if (agent == null)
            return Status.Failure;

        blackBoard = agent.BlackboardReference;
        if (blackBoard == null)
            return Status.Failure;


        string message = Message != null ? Message.Value : string.Empty;
        agent.Speak(message);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

