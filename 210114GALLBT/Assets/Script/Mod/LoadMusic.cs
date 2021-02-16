using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
public class LoadMusic : MonoBehaviour {
    string projectName;

    [SerializeField] public AudioSource S_ReelStop01, S_ReelStop02, S_ReelStop03, S_ReelStop04, S_ReelStop05, S_IncreaseBet01, S_IncreaseBet02, S_IncreaseBet03, S_IncreaseBet04, S_IncreaseBet05, S_IncreaseBet06, S_BonusAppear01, S_BonusAppear02, S_BonusAppear03, S_BonusAppear04, S_BonusAppear05, S_SpeedUp, S_Button, S_GoldCoinRolling, S_GoldCoinEnd, S_GoldCoinStart;
    static public AudioClip S_ReelStop01AC, S_ReelStop02AC, S_ReelStop03AC, S_ReelStop04AC, S_ReelStop05AC, S_IncreaseBet01AC, S_IncreaseBet02AC, S_IncreaseBet03AC, S_IncreaseBet04AC, S_IncreaseBet05AC, S_IncreaseBet06AC, S_BonusAppear01AC, S_BonusAppear02AC, S_BonusAppear03AC, S_BonusAppear04AC, S_BonusAppear05AC, S_SpeedUpAC, S_ButtonAC, S_GoldCoinRollingAC, S_GoldCoinEndAC, S_GoldCoinStartAC;

    [SerializeField] public AudioSource S_ReelBackground01, S_ReelBackground02, S_ReelBackground03, S_BonusGameBack, S_TransBGMusic, S_Bonus, S_Wild, S_P1, S_P2, S_P3, S_P4;
    static public AudioClip S_ReelBackground01AC, S_ReelBackground02AC, S_ReelBackground03AC, S_BonusGameBackAC, S_TransBGMusicAC, S_BonusAC, S_WildAC, S_P1AC, S_P2AC, S_P3AC, S_P4AC;
    //宙斯特殊音效
    [SerializeField] public AudioSource S_Swild, S_TransMusic;
    static public AudioClip S_SwildAC, S_TransMusicAC;
    //海神特殊音效
    [SerializeField] public AudioSource S_Multiple_x10x2, S_Multiple_x15x3, S_Multiple_x30x5, S_ChoiceBGMusic, S_BonusWins;
    static public AudioClip S_Multiple_x10x2AC, S_Multiple_x15x3AC, S_Multiple_x30x5AC, S_ChoiceBGMusicAC, S_BonusWinsAC;
    // Start is called before the first frame update

    void Start() {
        projectName = Mod_Data.projectName;
        LoadBGMusic();
        LoadGameMusic();

        initBGMusic();
        iniGameMusic();

        //遊戲特殊音效
        LoadGameSpecialMusic();
        iniGameSpecialMusic();

    }

    // Update is called once per frame
    void Update() {

    }

    //讀取system music
    void LoadBGMusic() {
        S_ReelStop01AC = LoadSystemMusicbyResource("System music", "S_ReelStop01");
        S_ReelStop02AC = LoadSystemMusicbyResource("System music", "S_ReelStop02");
        S_ReelStop03AC = LoadSystemMusicbyResource("System music", "S_ReelStop03");
        S_ReelStop04AC = LoadSystemMusicbyResource("System music", "S_ReelStop04");
        S_ReelStop05AC = LoadSystemMusicbyResource("System music", "S_ReelStop05");


        S_IncreaseBet01AC = LoadSystemMusicbyResource("System music", "S_IncreaseBet01");
        S_IncreaseBet02AC = LoadSystemMusicbyResource("System music", "S_IncreaseBet02");
        S_IncreaseBet03AC = LoadSystemMusicbyResource("System music", "S_IncreaseBet03");
        S_IncreaseBet04AC = LoadSystemMusicbyResource("System music", "S_IncreaseBet04");
        S_IncreaseBet05AC = LoadSystemMusicbyResource("System music", "S_IncreaseBet05");
        S_IncreaseBet06AC = LoadSystemMusicbyResource("System music", "S_IncreaseBet06");

        S_BonusAppear01AC = LoadSystemMusicbyResource("System music", "S_BonusAppear01");
        S_BonusAppear02AC = LoadSystemMusicbyResource("System music", "S_BonusAppear02");
        S_BonusAppear03AC = LoadSystemMusicbyResource("System music", "S_BonusAppear03");
        S_BonusAppear04AC = LoadSystemMusicbyResource("System music", "S_BonusAppear04");
        S_BonusAppear05AC = LoadSystemMusicbyResource("System music", "S_BonusAppear05");

        S_SpeedUpAC = LoadSystemMusicbyResource("System music", "S_SpeedUp");
        S_ButtonAC = LoadSystemMusicbyResource("System music", "S_Button");
        S_GoldCoinRollingAC = LoadSystemMusicbyResource("System music", "S_GoldCoinRolling");
        S_GoldCoinEndAC = LoadSystemMusicbyResource("System music", "S_GoldCoinEnd");
        S_GoldCoinStartAC = LoadSystemMusicbyResource("System music", "S_GoldCoinStart");
    }
    //初始化system music
    void initBGMusic() {
        S_ReelStop01.clip = S_ReelStop01AC;
        S_ReelStop02.clip = S_ReelStop02AC;
        S_ReelStop03.clip = S_ReelStop03AC;
        S_ReelStop04.clip = S_ReelStop04AC;
        S_ReelStop05.clip = S_ReelStop05AC;

        S_IncreaseBet01.clip = S_IncreaseBet01AC;
        S_IncreaseBet02.clip = S_IncreaseBet02AC;
        S_IncreaseBet03.clip = S_IncreaseBet03AC;
        S_IncreaseBet04.clip = S_IncreaseBet04AC;
        S_IncreaseBet05.clip = S_IncreaseBet05AC;
        S_IncreaseBet06.clip = S_IncreaseBet06AC;

        S_BonusAppear01.clip = S_BonusAppear01AC;
        S_BonusAppear02.clip = S_BonusAppear02AC;
        S_BonusAppear03.clip = S_BonusAppear03AC;
        S_BonusAppear04.clip = S_BonusAppear04AC;
        S_BonusAppear05.clip = S_BonusAppear05AC;

        S_SpeedUp.clip = S_SpeedUpAC;
        S_Button.clip = S_ButtonAC;
        S_GoldCoinRolling.clip = S_GoldCoinRollingAC;
        S_GoldCoinEnd.clip = S_GoldCoinEndAC;
        S_GoldCoinStart.clip = S_GoldCoinStartAC;
    }
    //讀取game music
    void LoadGameMusic() {
        S_ReelBackground01AC = LoadProjectMusicbyResource("Music", "S_ReelBackground01");
        S_ReelBackground02AC = LoadProjectMusicbyResource("Music", "S_ReelBackground02");
        S_ReelBackground03AC = LoadProjectMusicbyResource("Music", "S_ReelBackground03");

        S_BonusGameBackAC = LoadProjectMusicbyResource("Music", "S_BonusGameBack");
        S_TransBGMusicAC = LoadProjectMusicbyResource("Music", "S_TransBGMusic");

        S_BonusAC = LoadProjectMusicbyResource("Music", "S_Bonus");
        S_WildAC = LoadProjectMusicbyResource("Music", "S_Wild");
        S_P1AC = LoadProjectMusicbyResource("Music", "S_P1");
        S_P2AC = LoadProjectMusicbyResource("Music", "S_P2");
        S_P3AC = LoadProjectMusicbyResource("Music", "S_P3");
        S_P4AC = LoadProjectMusicbyResource("Music", "S_P4");

    }
    //初始化game music
    void iniGameMusic() {
        S_ReelBackground01.clip = S_ReelBackground01AC;
        S_ReelBackground02.clip = S_ReelBackground02AC;
        S_ReelBackground03.clip = S_ReelBackground03AC;

        S_BonusGameBack.clip = S_BonusGameBackAC;
        S_TransBGMusic.clip = S_TransBGMusicAC;

        S_Bonus.clip = S_BonusAC;
        S_Wild.clip = S_WildAC;
        S_P1.clip = S_P1AC;
        S_P2.clip = S_P2AC;
        S_P3.clip = S_P3AC;
        S_P4.clip = S_P4AC;
    }
    //讀取GameSpecial Music
    void LoadGameSpecialMusic() {
        S_TransMusicAC = LoadProjectMusicbyResource("Music", "S_TransMusic");
        S_SwildAC = LoadProjectMusicbyResource("Music", "S_Swild");
        S_Multiple_x10x2AC = LoadProjectMusicbyResource("Music", "S_Multiple x10x2");
        S_Multiple_x15x3AC = LoadProjectMusicbyResource("Music", "S_Multiple x15x3");
        S_Multiple_x30x5AC = LoadProjectMusicbyResource("Music", "S_Multiple x30x5");
        S_ChoiceBGMusicAC = LoadProjectMusicbyResource("Music", "S_ChoiceBGMusic");
        S_BonusWinsAC = LoadProjectMusicbyResource("Music", "S_BonusWins");
    }
    void iniGameSpecialMusic() {
        S_TransMusic.clip = S_TransMusicAC;
        S_Swild.clip = S_SwildAC;
        S_Multiple_x10x2.clip = S_Multiple_x10x2AC;
        S_Multiple_x15x3.clip = S_Multiple_x15x3AC;
        S_Multiple_x30x5.clip = S_Multiple_x30x5AC;
        S_ChoiceBGMusic.clip = S_ChoiceBGMusicAC;
        S_BonusWins.clip = S_BonusWinsAC;
    }

    //主程式-從Resource中讀取SystemMusic
    AudioClip LoadSystemMusicbyResource(string parentFoldname, string SystemMusicName) {
        AudioClip SystemMusic = Resources.Load<AudioClip>(parentFoldname + "/" + SystemMusicName);
        return SystemMusic;
    }
    //主程式-從Resource中讀取遊戲Music
    AudioClip LoadProjectMusicbyResource(string parentFoldname, string GameMusicName) {
        AudioClip GameMusic = Resources.Load<AudioClip>(projectName + "/" + parentFoldname + "/" + GameMusicName);
        return GameMusic;
    }
}
