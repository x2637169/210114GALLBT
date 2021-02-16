using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern_HSMediator;
public class RegisterMediator : MonoBehaviour
{
    Mod_Animation animCtrl;
    Mod_GameMath gameMath;
    Mod_MusicController musicController;
    Mod_UIController uiController;
    Mod_State state;
    Slots slots;
    Mod_State currentState;
    Mod_BonusScript bonusScript;
    ConcreteSlotMediator pMediator;
    BackEnd_Data backEndData;
    NewSramManager newSramManager;
    Mod_Client clientScript;
    Mod_GameButton gameButton;
    Mod_Animation_BTRule animation_BTRule;
    // Start is called before the first frame update
    void Start()
    {
        animCtrl = GetComponent<Mod_Animation>();//new AnimationController(pMediator);//
        gameMath = GetComponent<Mod_GameMath>();
        musicController = GetComponent<Mod_MusicController>();
        uiController = GetComponent<Mod_UIController>();
        slots = GameObject.Find("Slots").GetComponent<Slots>();
        bonusScript = GetComponent<Mod_BonusScript>();
        backEndData = GameObject.Find("BackEndManager").GetComponent<BackEnd_Data>();
        newSramManager = GameObject.Find("BackEndManager").GetComponent<NewSramManager>();
        clientScript = GameObject.Find("GameController").GetComponent<Mod_Client>();
        gameButton = FindObjectOfType<Mod_GameButton>();
        animation_BTRule = FindObjectOfType<Mod_Animation_BTRule>();

        switch (newSramManager.LoadStatus())// newSramManager.LoadStatus()
        {
            case 0:
                currentState = new BaseSpin();
                break;
            case 1:
                currentState = new BaseScrolling();
                break;
            case 2:
                currentState = new BonusTransIn();
                break;
            case 3:
                currentState = new BonusScrolling();
                break;
            default:
                currentState = new BaseSpin();
                break;
        }

        pMediator = new ConcreteSlotMediator(animCtrl, gameMath, musicController, uiController, slots, currentState, bonusScript, backEndData, clientScript, newSramManager, gameButton, animation_BTRule);
        backEndData.setMediator(pMediator);
        slots.setMediator(pMediator);
        animCtrl.setMediator(pMediator);
        gameMath.setMediator(pMediator);
        musicController.setMediator(pMediator);
        uiController.setMediator(pMediator);
        currentState.setMediator(pMediator);
        bonusScript.setMediator(pMediator);
        clientScript.setMediator(pMediator);
        gameButton.setMediator(pMediator);
        animation_BTRule.setMediator(pMediator);
    }
    private void Update()
    {
#if Server
        #region Server
        if (!Mod_Data.machineInit) return;
        #endregion
#endif

        currentState = currentState.Process();
        // //Debug.Log("Mod_Data.reelAllStop" + Mod_Data.reelAllStop);
        ////Debug.Log(currentState.stateName);
    }
    public void ReregisterState()//重新註冊目前"狀態(Mod_State)"class 
    {
        // pMediator = new ConcreteSlotMediator(animCtrl, gameMath, musicController, uiController, slots, currentState, bonusScript);

        pMediator.SetState(currentState);
        backEndData.setMediator(pMediator);
        slots.setMediator(pMediator);
        animCtrl.setMediator(pMediator);
        gameMath.setMediator(pMediator);
        musicController.setMediator(pMediator);
        uiController.setMediator(pMediator);
        currentState.setMediator(pMediator);
        bonusScript.setMediator(pMediator);
        clientScript.setMediator(pMediator);
        gameButton.setMediator(pMediator);
        animation_BTRule.setMediator(pMediator);
    }
}
