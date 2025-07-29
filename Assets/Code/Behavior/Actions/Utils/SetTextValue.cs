using System;
using TMPro;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Set Text Value",
        story: "Set [Text] to [Value]",
        category: "Action/Utils",
        id: "35eafb0f85583379c45d5cc361f84845")]
    public partial class SetTextValue : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Text;
        [SerializeReference] public BlackboardVariable Value;

        protected override Status OnStart()
        {
            if (Text.Value == null)
            {
                LogFailure("No Text assigned.");
                return Status.Failure;
            }

            var uiText = Text.Value.GetComponentInChildren<TMP_Text>();

            if (uiText == null)
            {
                LogFailure("Text don't have a valid Text component.");
                return Status.Failure;
            }

            uiText.text = Value.ObjectValue == null ? string.Empty : Value.ObjectValue.ToString();

            // We could use the OnUpdate to write the text over time...
            return Status.Success;
        }
    }
}