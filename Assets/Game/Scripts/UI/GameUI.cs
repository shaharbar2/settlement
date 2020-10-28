using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    [SerializeField] Button playerSettingsButton;
    [SerializeField] TMP_Text hintText;
    [SerializeField] public RadialMenu radialMenu;
    [SerializeField] private PlayerSettingsMenu playerSettingsMenu;

    private bool isRadialMenuVisible = false;

    void Awake() {
        playerSettingsButton.onClick.AddListener(onPlayerSettingsButtonClick);
        playerSettingsMenu.onClose += onPlayerSettingsMenuClose;
    }

    /// Public -- 

    public void setHintVisible(bool visible) {
        hintText.gameObject.SetActive(visible);
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

    /// Private -- 
    
    private void onPlayerSettingsButtonClick() {
        playerSettingsMenu.gameObject.SetActive(true);
        playerSettingsButton.gameObject.SetActive(false);
    }

    private void onPlayerSettingsMenuClose() {
        playerSettingsButton.gameObject.SetActive(true);
    }
}