using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Settlement.UI.RadialMenu {
    public class RadialMenuSegment {
        public float angle;
        public float minAngle;
        public float maxAngle;

        public bool isRoot;
        public List<RadialMenuSegment> children;

        public RadialMenuSegmentData data;
        // highlight animation data    
        public bool isHighlighted;
        public float highlightLerp;
        public LTDescr highlightTween;

        // selection animation data
        public bool isSelected;
        public float selectionLerp;
        public LTDescr selectionTween;

        public RadialMenuSegmentContent content;

        public RadialMenuSegment(RadialMenuSegmentContent image, float angle, float minAngle, float maxAngle) {
            this.content = image;
            this.angle = angle;
            this.minAngle = minAngle;
            this.maxAngle = maxAngle;
            children = new List<RadialMenuSegment>();
        }

        virtual public void configure(RadialMenuSegmentData data, bool isRoot) {
            this.data = data;
            this.isRoot = isRoot;
        }
    }
}