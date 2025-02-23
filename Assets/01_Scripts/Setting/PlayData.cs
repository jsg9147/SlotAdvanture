using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayData
{
    public int highestPercent;
    public int lowestPercent;

    public int monsterHighestPercent;
    public int monsterLowestPercent;

    public int hitCount;
    public int missCount;

    public int monsterHitCount;
    public int monsterMissCount;

    public int skillBookTryCount;
    public int skillBookSuccessCount;

    public int blackJackCount;
    public int blackJackWinCount;
    public int blackJackGetItemCount;

    public int ghostLegCount;
    public int ghostLegWinCount;

    public PlayData()
    {
        highestPercent = 0;
        lowestPercent = 100;

        hitCount = 0;
        missCount = 0;

        monsterHitCount = 0;
        monsterMissCount = 0;

        blackJackCount = 0;
        blackJackWinCount = 0;

        skillBookTryCount = 0;
        skillBookSuccessCount = 0;

        blackJackCount = 0;
        blackJackWinCount = 0;
        blackJackGetItemCount = 0;

        ghostLegCount = 0;
        ghostLegWinCount = 0;
    }

    public void TrySkillBook(bool isSuccess)
    {
        skillBookTryCount++;
        if (isSuccess)
            skillBookSuccessCount++;

        if (skillBookTryCount > skillBookSuccessCount && skillBookSuccessCount > 0)
        {
            SteamAchievements.Instance.HandleBookLoverAchievement();
        }
    }

    public void SetAttackRecord(Belong belong, bool isSuccess)
    {
        if (belong == Belong.Player)
        {
            if (isSuccess)
                hitCount++;
            else
                missCount++;
        }
        else
        {
            if (isSuccess)
                monsterHitCount++;
            else
                monsterMissCount++;
        }
    }

    public void SetSlotResult(Belong belong, int result)
    {
        if (belong == Belong.Player)
        {
            if (highestPercent <= result)
                highestPercent = result;
            if (lowestPercent >= result)
                lowestPercent = result;
        }
        else
        {
            if (monsterHighestPercent <= result)
                monsterHighestPercent = result;
            if (monsterLowestPercent >= result)
                monsterLowestPercent = result;
        }
    }

    public void BlackJackStart() => blackJackCount++;
}
