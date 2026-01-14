using System;
using UnityEngine;

public static class OfflineRewardData
{
    public static void SaveExitTime(TimeSpan accumulatedDuration) //cikarkenki zamani kaydet
    {
        string timeString = DateTime.UtcNow.ToBinary().ToString(); //zaman dilimini binary formatta al

        PlayerPrefs.SetString(PlayerPrefsKeys.LAST_EXIT_TIME, timeString);
        PlayerPrefs.SetInt(PlayerPrefsKeys.HAS_SAVED_DATA, 1);
        PlayerPrefs.SetString(PlayerPrefsKeys.ACCUMULATED_DURATION_TICKS, accumulatedDuration.Ticks.ToString());
        PlayerPrefs.Save();
    }

    public static DateTime? LoadExitTime() //son cikis zamanini getir
    {
        if (PlayerPrefs.GetInt(PlayerPrefsKeys.HAS_SAVED_DATA, 0) == 0) return null;

        string timeString = PlayerPrefs.GetString(PlayerPrefsKeys.LAST_EXIT_TIME, string.Empty);

        if (string.IsNullOrEmpty(timeString)) return null;

        if (long.TryParse(timeString, out long binary))
        {
            return DateTime.FromBinary(binary);
        }

        return null;
    }

    public static TimeSpan? GetOfflineDuration() //oyuncunun ne kadar sure offline kaldigini hesapla
    {
        DateTime? lastExit = LoadExitTime();

        if (!lastExit.HasValue) return null;

        TimeSpan duration = DateTime.UtcNow - lastExit.Value;

        if (duration.TotalSeconds < 0) return null;

        return duration;
    }

    public static TimeSpan LoadAccumulatedDuration()
    {
        string ticksString = PlayerPrefs.GetString(PlayerPrefsKeys.ACCUMULATED_DURATION_TICKS, string.Empty);

        if (string.IsNullOrEmpty(ticksString)) return TimeSpan.Zero;

        if (long.TryParse(ticksString, out long ticks))
        {
            return TimeSpan.FromTicks(ticks);
        }

        return TimeSpan.Zero;
    }

    public static float GetCurrentCycleProgress(TimeSpan offlineDuration, float cycleDuration) //suanki dongu ilerlemesini hesapla (progres bar icin)
    {
        float secondsInCycle = (float)(offlineDuration.TotalSeconds % cycleDuration);
        return secondsInCycle / cycleDuration;
    }

    public static int GetCompletedCycles(TimeSpan offlineDuration, float cycleDuration) //offline iken kac tane tam dongu tamamlanmis
    {
        return (int)Math.Floor(offlineDuration.TotalSeconds / cycleDuration);
    }

    public static void ClearData()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKeys.LAST_EXIT_TIME);
        PlayerPrefs.DeleteKey(PlayerPrefsKeys.HAS_SAVED_DATA);
        PlayerPrefs.DeleteKey(PlayerPrefsKeys.ACCUMULATED_DURATION_TICKS);
        PlayerPrefs.Save();

        Debug.Log("Data cleared.");
    }
}
