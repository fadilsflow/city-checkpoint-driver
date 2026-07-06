using UnityEngine;

public static class SaveManager
{
    private const string UnlockedLevelKey = "UnlockedLevel";

    public static int UnlockedLevel
    {
        get => Mathf.Max(1, PlayerPrefs.GetInt(UnlockedLevelKey, 1));
        set
        {
            PlayerPrefs.SetInt(UnlockedLevelKey, Mathf.Max(value, UnlockedLevel));
            PlayerPrefs.Save();
        }
    }

    public static float GetBestTime(int levelIndex)
    {
        return PlayerPrefs.GetFloat($"Level_{levelIndex}_BestTime", 0f);
    }

    public static int GetBestStars(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level_{levelIndex}_BestStars", 0);
    }

    public static void SaveLevelResult(int levelIndex, float finishTime, int stars)
    {
        float bestTime = GetBestTime(levelIndex);
        if (bestTime <= 0f || finishTime < bestTime)
            PlayerPrefs.SetFloat($"Level_{levelIndex}_BestTime", finishTime);

        if (stars > GetBestStars(levelIndex))
            PlayerPrefs.SetInt($"Level_{levelIndex}_BestStars", stars);

        if (levelIndex >= UnlockedLevel)
            PlayerPrefs.SetInt(UnlockedLevelKey, levelIndex + 1);

        PlayerPrefs.Save();
    }

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    public static float MusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 0.35f);

    public static float SfxVolume => PlayerPrefs.GetFloat(SfxVolumeKey, 0.7f);

    public static void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(volume));
        PlayerPrefs.Save();
    }

    public static void SetSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(volume));
        PlayerPrefs.Save();
    }
}
