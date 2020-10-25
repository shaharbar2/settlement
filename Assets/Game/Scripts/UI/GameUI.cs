using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour {

    [SerializeField] TMP_Text hintText;
    [SerializeField] public RadialMenu radialMenu;
    
    private bool isRadialMenuVisible = false;
    private float radialMenuHoldElapsed = 0f;
    
    void Update() {
        handleRadialMenuInput();
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
    
    private void handleRadialMenuInput() {
        KeyCode keyCode = Constants.instance.BM_KEY_CODE;
        if (Input.GetKeyDown(keyCode)) {
            radialMenuHoldElapsed = 0;
        } 

        if (Input.GetKey(keyCode)) {
            radialMenuHoldElapsed += Time.deltaTime;
            if (radialMenuHoldElapsed >= Constants.instance.BM_KEY_HOLD) {
                showRadialMenu();
            }
        }
        if (Input.GetKeyUp(keyCode)) {
            hideRadialMenu();
        }
    }
}