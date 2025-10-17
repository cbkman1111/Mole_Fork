using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public partial class WorldObject : StateMachine
    {
        private InteractionType InteractionType = InteractionType.None;

        public void OnReciveInteraction(InteractionType type)
        {
            InteractionType = type;

            MEC.Timing.RunCoroutine(HandleInteraction().CancelWith(gameObject));
        }

        private IEnumerator<float> HandleInteraction()
        {
            //yield return MEC.Timing.WaitForOneFrame;
            yield return MEC.Timing.WaitForSeconds(1f);

            switch (InteractionType)
            {
                case InteractionType.Talk:
                    Speak("그래 안녕~ 반가워~");
                    break;
            }

            InteractionType = InteractionType.None;
            yield return MEC.Timing.WaitForSeconds(1f);

            Speak(string.Empty);
        }
    }
}