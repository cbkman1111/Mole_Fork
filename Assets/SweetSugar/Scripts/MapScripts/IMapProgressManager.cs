namespace SweetSugar.Scripts.MapScripts
{
    public interface IMapProgressManager
    {
        int LoadLevelStarsCount(int level);
        void SaveLevelStarsCount(int level, int starsCount);
        void ClearLevelProgress(int level);

        int GetLastLevel();
        string GetLevelKey(int number);
        string GetScoreKey(int number);
        void SaveLevelStarsCount(int level, int starsCount, int score);
    }
}
