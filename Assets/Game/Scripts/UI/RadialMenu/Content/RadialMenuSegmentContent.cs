using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Settlement.UI.RadialMenu {
    public class RadialMenuSegmentContent : MonoBehaviour {
        [HideInInspector] public Image background;
        
        protected virtual void Awake() {
            background = GetComponent<Image>();
        }

        virtual public void configure(RadialMenuSegmentData data) {
            
        }

        virtual public void animateHighlight(float lerp) {

        }

        virtual public void animateUpdate(float lerp, Vector3 center) {

        }
    }
}