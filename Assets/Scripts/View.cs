using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    public Text
        tournamentNameText,
        levelText,
        timerText,
        blindsText,
        anteText,
        playersText,
        avgText,
        nextBreakText,
        nextBlindsText;
    public Button
        plusButton,
        minusButton,
        pauseButton;

    [SerializeField]
    Sprite
        pauseSprite,
        playSprite;

    public void DisplayTournamentText(string tournamentName)
    {
        tournamentNameText.text = tournamentName;
    }

    public void OnLevelChanged(int level,int smallBlind,int bigBlind,int bbAnte,int nextSmallBlind,int nextBigBilnd,int nextBBAnte)
    {
        levelText.text = "Level " + (level+1).ToString();
        blindsText.text = smallBlind.ToString() + "/" + bigBlind.ToString();
        anteText.text = bbAnte.ToString();
        nextBlindsText.text = "Next Blinds (Ante) : " + nextSmallBlind.ToString() + "/" + nextBigBilnd.ToString() + "(" + nextBBAnte.ToString() + ")";
    }

    public void OnTimerChanged(int timer)
    {
        var minute = timer / 60;
        var seconds = timer % 60;
        timerText.text = minute.ToString() + ":" + seconds.ToString("00");
    }

    public void DisplayAvgStack(int stack)
    {
        avgText.text = stack.ToString();
    }

    public void DisplayPlayerNumberText(int remainPlayerNum,int startPlayerNum)
    {
        playersText.text = remainPlayerNum.ToString() + "/" + startPlayerNum.ToString(); 
    }

    public void OnBreakCalled()
    {
        levelText.text = "休憩中";
    }

    public void ChangePauseButtonSprite(State state)
    {
        switch (state)
        {
            case State.PAUSE:
                pauseButton.image.sprite = playSprite;
                break;
            case State.PLAY:
                pauseButton.image.sprite = pauseSprite;
                break;
            default:
                break;
        }
    }
}
