using System;
using Unity.Behavior;

namespace Creature
{
	[BlackboardEnum]
    public enum ObjectActionState
    {
        None = 0,
        Idle,

        Chase, // 추적.
        Patrol, // 자동 순찰.
        Move, // 사용자의 직접 이동.

        Stop,
        Hit,
        Attack,
        Dead,

        Click, // 클릭했을때 테스트용.
    }

    [BlackboardEnum]
    public enum InteractionType
    {
        None = 0,
        Talk,
    }
}