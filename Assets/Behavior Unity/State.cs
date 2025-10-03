using System;
using Unity.Behavior;

namespace Creature
{
	[BlackboardEnum]
	public enum WorldObjectState
	{
        Die = 0, // 죽음 상태

        Idle, // 일반 상태
		Patrol, // 순찰 상태
        Chase, // 추적 상태
        Attack, // 공격 상태
        Eat, // 먹기 상태
        Sleep, // 잠자기 상태

        Max
    }
}