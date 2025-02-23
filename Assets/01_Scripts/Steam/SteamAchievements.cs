using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{
    private static SteamAchievements instance;

    private const string ACHIEVEMENT_GAME_START = "GAME_START";
    private const string ACHIEVEMENT_SLIME_KILLER = "SLIME_KILLER";
    private const string ACHIEVEMENT_BOSS_SLAYER = "BOSS_SLAYER";
    private const string ACHIEVEMENT_UNLUCKY_START = "UNLUCKY_START";
    private const string ACHIEVEMENT_GOOD_LUCK = "GOOD_LUCK";
    private const string ACHIEVEMENT_HIT_999 = "HIT_999";
    private const string ACHIEVEMENT_GAMBLER = "GAMBLER";
    private const string ACHIEVEMENT_DIFFERENCE_OF_STARS = "DIFFERENCE_OF_STARS";
    private const string ACHIEVEMENT_REVIVAL = "REVIVAL";
    private const string ACHIEVEMENT_ALL_EQUIPPED = "ALL_EQUIPPED";
    private const string ACHIEVEMENT_COMEBACK = "COMEBACK";
    private const string ACHIEVEMENT_BOOK_LOVER = "BOOK_LOVER";

    int slimeKillCount = 0;


    public static SteamAchievements Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        Initialized();
    }

    void Initialized()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        // Steam API 초기화
        if (!SteamManager.Initialized)
        {
            Application.Quit();
            Debug.LogError("Steamworks not initialized. Achievements will not work.");
            return;
        }

        // 첫 실행 도전과제 확인
        CheckFirstAchievement();
    }

    private void UnlockAchievement(string achievementId)
    {
        SteamUserStats.SetAchievement(achievementId);
        SteamUserStats.StoreStats();
    }

    public void GetArchievement(string achievementId)
    {
        bool isUnlock = SteamUserStats.GetAchievement(achievementId, out bool unlocked);

        if (!unlocked)
        {
            // 도전과제가 처음으로 달성되면 업적 언락
            UnlockAchievement(achievementId);
            Debug.Log("첫 번째 도전과제 달성!");
        }
    }

    public void ScoreReset()
    {
        slimeKillCount = 0;
    }

    #region 언락 코드들
    private void CheckFirstAchievement()
    {
        bool isFirstAchievementUnlocked = SteamUserStats.GetAchievement(ACHIEVEMENT_GAME_START, out bool unlocked);

        if (!unlocked)
        {
            // 도전과제가 처음으로 달성되면 업적 언락
            UnlockAchievement(ACHIEVEMENT_GAME_START);
            Debug.Log("첫 번째 도전과제 달성!");
        }
    }

    public void AddSlimeKill()
    {
        slimeKillCount++;
        CheckSlimeAchievement();
    }
    // 슬라임 킬 업적
    private void CheckSlimeAchievement()
    {
        if (slimeKillCount >= 10)
            UnlockAchievement(ACHIEVEMENT_SLIME_KILLER);
    }
    // 게임 클리어 업적
    public void GameClearAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_BOSS_SLAYER);
    }

    // 불행의 시작 업적
    public void HandleUnluckyStartAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_UNLUCKY_START);
    }

    // 굿럭 or 배드럭 업적
    public void HandleGoodLuckAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_GOOD_LUCK);
    }

    // 반드시 때린다! 업적
    public void HandleHit999Achievement()
    {
        UnlockAchievement(ACHIEVEMENT_HIT_999);
    }

    // 겜블러 업적
    public void HandleGamblerAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_GAMBLER);
    }

    public void BuffStarsCheck()
    {
        for (int i = 0; i < GameManager.Instance.playerUnitDatas.Length; i++)
        {

        }
    }
    // 별의 차이 업적
    public void HandleDifferenceOfStarsAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_DIFFERENCE_OF_STARS);
    }

    // 리바이벌! 업적
    public void HandleRevivalAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_REVIVAL);
    }

    // 전투 준비 완료 업적
    public void HandleAllEquippedAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_ALL_EQUIPPED);
    }

    // 일발 역전 업적
    public void HandleComebackAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_COMEBACK);
    }

    // 나는 독서왕?! 업적
    public void HandleBookLoverAchievement()
    {
        UnlockAchievement(ACHIEVEMENT_BOOK_LOVER);
    }

    #endregion

#if UNITY_EDITOR
    public void ResetAllStats()
    {
        bool isReset = SteamUserStats.ResetAllStats(true);
    }
#endif

}


