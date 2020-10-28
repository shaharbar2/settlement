using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour {

    [SerializeField] TMP_Text hintText;
    [SerializeField] public RadialMenu radialMenu;
    
    private bool isRadialMenuVisible = false;
    
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
}