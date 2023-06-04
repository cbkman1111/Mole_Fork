using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public class DebugSettings : ScriptableObject
    {
        public bool BonusCombinesShowLog;
        public bool DestroyLog;
        public bool FallingLog;
        public bool StackTrace;
        public bool ShowLogImmediately;
        [Header("AI testing options")]
        [Tooltip("Enable AI player")]
        public bool AI;
        [Tooltip("Level for testing from the map")]
        public int testLevel;
        [Tooltip("Non-gameplay UI animations speed")]
        [Range(0, 100)] public float TimeScaleUI = 1;
        [Tooltip("Gameplay animations speed")]
        [Range(0, 100)] public float TimeScaleItems = 1;

        [Header("Debug hotkeys")] public bool enableHotkeys = true;
        [Tooltip("press to win")] public KeyCode Win;
        [Tooltip("set moves to 1")] public KeyCode Lose;
        [Tooltip("restart the level")] public KeyCode Restart;
        [Tooltip("switch a sublevel")] public KeyCode SubSwitch;
        [Tooltip("android's back button")] public KeyCode Back;
        [Tooltip("regenerate items")] public KeyCode Regen;

        [Header("")] [Tooltip("Test language, only for editor")]
        public SystemLanguage TestLanguage = SystemLanguage.English;


    }
}