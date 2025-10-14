using System;
using Unity.Behavior;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SerchRange", 
    story: "SerchRange [Agent] Check Skill Range [Enemy]", 
    category: "Action", 
    id: "bf1918c16e89190eb534a59b4208c9f1")]
public partial class SerchRangeAction : CretureAction
{
    protected override Status OnUpdate()
    {
        
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

