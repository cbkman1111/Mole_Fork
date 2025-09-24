using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
    IDLE,
	PATROL,
	ATTACK,
	DIE,
	MOVE,
	EAT,
	SLEEP,
	CHASE
}
