using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settlement.UI.RadialMenu {
    public class BuildMenuSegmentContent : RadialMenuSegmentContent {
        [SerializeField] public GameObject container;
        [SerializeField] public TMP_Text text;
        [SerializeField] public RawImage icon;
        
        [SerializeField] public RectTransform cost;
        [SerializeField] public RawImage costIcon;
        [SerializeField] public TMP_Text costText;

        protected override void Awake() {
            base.Awake();
        }

        public override void configure(RadialMenuSegmentData data) {
            base.configure(data);

            var buildSegmentData = ((BuildMenuSegmentData)data);
            var buildData = BuildingConfiguration.instance.buildingDataFor(buildSegmentData.buildingType);
            icon.texture = buildSegmentData.iconTexture;

            if (buildData.type == BuildingType.Undefined) {
                text.text = buildSegmentData.name;
                costText.text = "";
                costIcon.gameObject.SetActive(false);
            } else {
                text.text = buildData.name;
                costText.text = buildData.costToConstruct.ToString();
                costIcon.gameObject.SetActive(true);
            }
        }

        public override void animateHighlight(float lerp) {
            base.animateHighlight(lerp);
            icon.transform.localScale = Vector3.one * (1f + 0.5f * lerp);
        }

        public override void animateUpdate(float lerp, Vector3 center) {
            base.animateUpdate(lerp, center);

            container.GetComponent<RectTransform>().anchoredPosition = center;
            text.color = text.color.setAlpha(lerp);
            icon.color = icon.color.setAlpha(lerp);
            costText.color = costText.color.setAlpha(lerp);
            costIcon.color = costIcon.color.setAlpha(lerp);

            cost.anchoredPosition = center * -0.3f;
        }
    }
}