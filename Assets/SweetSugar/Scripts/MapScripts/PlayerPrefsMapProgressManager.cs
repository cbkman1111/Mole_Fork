
using UnityEngine;

namespace SweetSugar.Scripts.MapScripts
{
    public class PlayerPrefsMapProgressManager : IMapProgressManager
    {
        private int currentLevel;
        private int tempsaveCounter;

        public int CurrentLevel
        {
            get
            {
                if (currentLevel == 0) currentLevel = GetLastLevel();
                return currentLevel;
            }
            set => currentLevel = value;
        }

        public PlayerPrefsMapProgressManager()
        {
        }

        public string GetLevelKey(int number)
        {
            return string.Format("Level.{0:000}.StarsCount", number);
        }

        public string GetScoreKey(int number)
        {
            return string.Format("Level.{0:000}.Score", number);
        }

        public int LoadLevelStarsCount(int level)
        {
            if (level == 0) return 1;
            return PlayerPrefs.GetInt(GetLevelKey(level), 0);
        }

        public void SaveLevelStarsCount(int level, int starsCount)
        {
            PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
            if (GetLastLevel() < level) PlayerPrefs.SetInt("LastLevel", level);
            PlayerPrefs.Save();
        }

        public void SaveLevelStarsCount(int level, int starsCount, int score)
        {
            PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
            PlayerPrefs.SetInt(GetScoreKey(level), score);
            if (GetLastLevel() < level) PlayerPrefs.SetInt("LastLevel", level);
            PlayerPrefs.Save();
        }

        public void ClearLevelProgress(int level)
        {
            PlayerPrefs.DeleteKey(GetLevelKey(level));
        }

        public int GetLastLevel()
        {
            while (LoadLevelStarsCount(tempsaveCounter) > 0)
            {
                tempsaveCounter++;
            }

            return tempsaveCounter;
        }

        public void OpenAllLevels()
        {
            for (int i = 1; i < 1000; i++)
            {
                SaveLevelStarsCount(i, 3);
            }
        }
    }
}
