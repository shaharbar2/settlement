using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Settlement.UI.RadialMenu {
    public class RadialMenuConfig : MonoBehaviour {
        [HideInInspector] private RadialMenuSegmentData[] _data;

        public RadialMenuSegmentData[] data { get {
            if (_data == null) {
                initialize();
            }
            return _data;
        }}

        void Awake() {
            if (_data == null) {
                initialize();
            }
        }

        private void initialize() {
            var all = GetComponentsInChildren<RadialMenuSegmentData>();
            var root = new List<RadialMenuSegmentData>();
            foreach (var data in all) {
                var children = data.gameObject.GetComponentsInChildren<RadialMenuSegmentData>();
                if (children.Length > 1) {
                    root.Add(data);
                    data.children = new RadialMenuSegmentData[children.Length - 1];
                    for(int i = 1; i < children.Length; i++) {
                        data.children[i - 1] = children[i];
                    }
                }
            }

            _data = root.ToArray();
        }
    }
}