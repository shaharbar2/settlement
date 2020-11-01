using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    [SerializeField] private Button playerSettingsButton;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] public RadialMenu radialMenu;
    [SerializeField] private PlayerSettingsMenu playerSettingsMenu;

    private bool isRadialMenuVisible = false;

    public bool blockPlayerMouseMovement = false;
    
    void Awake() {
        playerSettingsButton.onClick.AddListener(onPlayerSettingsButtonClick);
        playerSettingsMenu.onClose += onPlayerSettingsMenuClose;
    }

    /// Public -- 

    public void showHint(string text) {
        hintText.gameObject.SetActive(true);
        hintText.text = text;
    }

    public void hideHint() {
        hintText.gameObject.SetActive(false);
    }

    public void showRadialMenu() {
        if (!isRadialMenuVisible) {
            radialMenu.showAnimated();
            isRadialMenuVisible = true;
        }
    }

    public void hideRadialMenu() {
        if (isRadialMenuVisible) {
            radialMenu.hideAnimated();
            isRadialMenuVisible = false;
        }
    }

    public void setCoinsAmount(int amount) {
        coinsText.text = amount + "";
    }

    /// Private -- 
    
    private void onPlayerSettingsButtonClick() {
        playerSettingsMenu.gameObject.SetActive(true);
        playerSettingsButton.gameObject.SetActive(false);
    }

    private void onPlayerSettingsMenuClose() {
        playerSettingsButton.gameObject.SetActive(true);
    }
}