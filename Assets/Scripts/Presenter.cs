﻿using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Presenter : MonoBehaviour
{
    [SerializeField]
    List<BlindsData> blindsList = new List<BlindsData>();

    [SerializeField]
    View view;

    ReactiveProperty<int> level = new ReactiveProperty<int>();
    ReactiveProperty<int> timer = new ReactiveProperty<int>();

    [SerializeField]
    [Header("トーナメント名")]
    string tournamentName;

    [SerializeField]
    [Header("スタートスタック")]
    int startStack;

    [SerializeField]
    [Header("スタートプレイヤー数")]
    int startPlayerNumber;
    ReactiveProperty<int> remainedPlayerNumber = new ReactiveProperty<int>();

    State currentState = State.PLAY;

    void Awake()
    {
        SetDefaultValue();

        view.DisplayTournamentText(tournamentName);
        view.DisplayAvgStack(startStack);

        level.ObserveEveryValueChanged(_ => _.Value)
            .Subscribe(_ => view.OnLevelChanged(
                level.Value,
                blindsList[level.Value].smallBlind,
                blindsList[level.Value].bigBlind,
                blindsList[level.Value].bbAnte,
                blindsList[level.Value + 1].smallBlind,
                blindsList[level.Value + 1].bigBlind,
                blindsList[level.Value + 1].bbAnte
            ));

        timer.ObserveEveryValueChanged(_ => _.Value)
            .Subscribe(_ => view.OnTimerChanged(timer.Value)).AddTo(gameObject);

        timer.ObserveEveryValueChanged(_ => _.Value)
            .Where(_ => timer.Value <= 0)
            .Subscribe(_ => PlusLevel()).AddTo(gameObject);

        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(
            _ => timer.Value--
            ).AddTo(gameObject);

        remainedPlayerNumber.ObserveEveryValueChanged(_ => _.Value)
            .Subscribe(_ =>
            {
                var avgStack = startStack * ((float)startPlayerNumber / remainedPlayerNumber.Value);
                view.DisplayAvgStack((int)avgStack);
                view.DisplayPlayerNumberText(remainedPlayerNumber.Value,startPlayerNumber);
            }).AddTo(gameObject);

        SetEvents();
    }

    void SetEvents()
    {
        view.plusButton.onClick.AddListener(OnPlusButtonClicked);
        view.minusButton.onClick.AddListener(OnMinusButtonClicked);
        view.pauseButton.onClick.AddListener(OnPauseButtonClicked);
    }

    void SetDefaultValue()
    {
        level.Value = 0;
        SetTimer(blindsList[level.Value].timer * 60);
        remainedPlayerNumber.Value = startPlayerNumber;
    }

    void PlusLevel()
    {
        level.Value++;
        SetTimer(blindsList[level.Value].timer * 60);
        var se = (AudioClip)Resources.Load("blindUpSound");
        GetComponent<AudioSource>().PlayOneShot(se);
    }

    void SetTimer(int time)
    {
        this.timer.Value = time;
    }

    void OnPlusButtonClicked()
    {
        if (remainedPlayerNumber.Value + 1 > startPlayerNumber) return;
        remainedPlayerNumber.Value++;
    }

    void OnMinusButtonClicked()
    {
        if (remainedPlayerNumber.Value - 1 < 1) return;
        remainedPlayerNumber.Value--;
    }

    void OnPauseButtonClicked()
    {
        switch (currentState)
        {
            case State.PAUSE:
                currentState = State.PLAY;
                Time.timeScale = 1;
                view.ChangePauseButtonSprite(currentState);
                break;
            case State.PLAY:
                currentState = State.PAUSE;
                Time.timeScale = 0;
                view.ChangePauseButtonSprite(currentState);
                break;
            default:
                break;
        }
    }
}

public enum State
{
    PLAY,
    PAUSE
}
