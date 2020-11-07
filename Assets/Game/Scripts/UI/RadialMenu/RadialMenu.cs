using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settlement.UI.RadialMenu {
    public class RadialMenu : MonoBehaviour {
        [HideInInspector] public delegate void RadialMenuEvent(RadialMenuSegmentData data);
        [HideInInspector] public event RadialMenuEvent onClicked;
        [HideInInspector] public event RadialMenuEvent onHighlighted;

        /// Private -- 

        [SerializeField] private float segmentRadius;
        [SerializeField] private float segmentSpace;
        [SerializeField] private RadialMenuSegmentContent segmentReference;

        [SerializeField] private float rootCenterRadius;
        [SerializeField] private float childCenterRadius;

        private RadialMenuSegmentData[] data;
        private List<RadialMenuSegment> segments = new List<RadialMenuSegment>();

        private LTDescr showTween;
        private LTDescr hideTween;

        private float mouseAngle = 0;
        private float mouseDistance = 0;
        private float angle = 360;
        private float lerp = 0;

        private RadialMenuSegment selectedRootSegment;
        private RadialMenuSegment selectedChildSegment;
        private RadialMenuSegment clickedSegment;

        void Awake() {
            data = GetComponentInChildren<RadialMenuConfig>().data;
            createSegments();
            segmentReference.gameObject.SetActive(false);
        }

        void Update() {
            selectedRootSegment = updateMouseSelection(segments);
            if (Input.GetMouseButtonDown(0) && selectedChildSegment != null) {
                onClicked?.Invoke(selectedChildSegment.data);
                clickedSegment = selectedChildSegment;
                GameObject go = selectedChildSegment.content.gameObject;
                LeanTween.scale(go, Vector3.one * 1.8f, 1f).setOnComplete(() => {
                    go.transform.localScale = Vector3.one;
                }).setEase(LeanTweenType.easeOutQuad);
                _hideAnimated();
            }
        }

        /// Public -- 

        public void showAnimated() {
            clickedSegment = null;

            if (hideTween != null) {
                LeanTween.cancel(hideTween.id);
                hideTween = null;
            }
            if (showTween == null) {
                float time = Constants.instance.BM_TIME_SHOW * (1f - lerp);
                LeanTweenType ease = Constants.instance.BM_SHOW_EASE;
                showTween = LeanTween.value(gameObject, lerp, 1f, time).setEase(ease).setOnUpdate((float value) => {
                    this.lerp = value;
                    updateRootSegments(lerp: value);
                }).setOnComplete(() => {
                    showTween = null;
                });
            }
        }

        public void hideAnimated() {
            if (hideTween == null && lerp > 0) {
                onClicked?.Invoke(null);
                _hideAnimated();
            }
        }

        public void _hideAnimated() {
            if (showTween != null) {
                LeanTween.cancel(showTween.id);
                showTween = null;
            }
            if (hideTween == null) {
                selectedRootSegment = null;
                selectedChildSegment = null;
                float time = Constants.instance.BM_TIME_HIDE * lerp;
                LeanTweenType ease = Constants.instance.BM_HIDE_EASE;
                hideTween = LeanTween.value(gameObject, lerp, 0, time).setEase(ease).setOnUpdate((float value) => {
                    this.lerp = value;
                    updateRootSegments(lerp: value);
                }).setOnComplete(() => {
                    hideTween = null;
                });
            }
        }

        /// Private -- 

        private RadialMenuSegment updateMouseSelection(List<RadialMenuSegment> segments) {
            if (lerp >= 0.8f) {
                mouseAngle = BabyUtils.InvertAngle(BabyUtils.VectorAngle(Input.mousePosition, BabyUtils.screenBoundsPixels.center));
                mouseDistance = Vector2.Distance(BabyUtils.screenBoundsPixels.center, Input.mousePosition);
            } else if (lerp < 0.8f) {
                // deselect all when hidden
                mouseAngle = -1000;
                mouseDistance = 1000;
                selectedRootSegment = null;
                selectedChildSegment = null;
            }
            RadialMenuSegment selectedSegment = null;
            foreach (RadialMenuSegment segment in segments) {
                bool mouseOver = segment.minAngle < mouseAngle && segment.maxAngle > mouseAngle;
                if (mouseOver) {
                    if (!segment.isHighlighted && hideTween == null) {
                        segment.isHighlighted = true;
                        animateSegmentHighlight(segment);
                        if (!segment.isRoot) {
                            onHighlighted?.Invoke(segment.data);
                        }
                    }

                    selectedSegment = segment;
                } else {
                    if (segment.isHighlighted) {
                        segment.isHighlighted = false;
                        animateSegmentHighlight(segment);
                    }
                }
                if (segment == selectedSegment) {
                    if (segment.isRoot) {
                        selectedChildSegment = updateMouseSelection(segment.children);
                    }
                    if (!segment.isSelected) {
                        segment.isSelected = true;
                        animateSegmentSelected(segment);
                    }
                } else {
                    if (segment.isRoot) {
                        updateMouseSelection(segment.children);
                    }
                    if (segment.isSelected) {
                        segment.isSelected = false;
                        animateSegmentSelected(segment);
                    }
                }

            }
            return selectedSegment;
        }

        private void animateSegmentSelected(RadialMenuSegment segment) {
            if (segment.selectionTween != null) {
                LeanTween.cancel(segment.selectionTween.id);
                segment.selectionTween = null;
            }
            float from = segment.selectionLerp;
            float to = segment.isSelected ? 1f : 0f;
            float time = Constants.instance.BM_TIME_CHILD_SHOW;
            LeanTweenType easing = Constants.instance.BM_SHOW_EASE;
            if (!segment.isSelected) {
                if (clickedSegment == null) {
                    time = Constants.instance.BM_TIME_CHILD_HIDE;
                    easing = Constants.instance.BM_SHOW_EASE;
                } else {
                    time = Constants.instance.BM_TIME_CLICKED_CHILD_HIDE;
                    easing = Constants.instance.BM_CLICKED_CHILD_HIDE_EASE;
                }
            }
            segment.selectionTween = LeanTween.value(segment.content.gameObject, from, to, time).setOnUpdate((float lerp) => {
                segment.selectionLerp = lerp;
                for (int i = 0; i < segment.children.Count; i++) {
                    updateChildSegment(segment.children[i], i, lerp, segment.angle, segment.children.Count);
                }
            }).setOnComplete(() => {
                segment.selectionTween = null;
            }).setEase(easing);
        }

        private void animateSegmentHighlight(RadialMenuSegment segment) {
            if (segment.highlightTween != null) {
                LeanTween.cancel(segment.highlightTween.id);
                segment.highlightTween = null;
            }
            float from = segment.highlightLerp;
            float to = segment.isHighlighted ? 1f : 0f;
            float time = Constants.instance.BM_TIME_HIGHLIGHT;
            segment.highlightTween = LeanTween.value(segment.content.gameObject, from, to, time).setOnUpdate((float lerp) => {
                segment.highlightLerp = lerp;
                Color color = Color.Lerp(Constants.instance.BM_BASE_COLOR, Constants.instance.BM_SELECTED_COLOR, lerp);
                segment.content.background.material.SetColor("_FillColor", color);
                segment.content.animateHighlight(lerp);
            }).setOnComplete(() => {
                segment.highlightTween = null;
            });
        }

        private void updateRootSegments(float lerp) {
            for (int i = 0; i < segments.Count; i++) {
                RadialMenuSegment segment = segments[i];
                updateRootSegment(segment, idx : i, lerp : lerp);
            }
        }

        private void createSegments() {
            for (int i = 0; i < data.Length; i++) {
                RadialMenuSegmentContent content = Instantiate(segmentReference, Vector3.zero, Quaternion.identity, transform);
                Image image = content.background;
                image.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                image.material = new Material(image.material);

                float stepAngle = angle / data.Length;
                float minAngle = i * stepAngle;
                float segAngle = minAngle + stepAngle / 2f;
                float textRadius = image.rectTransform.rect.width * rootCenterRadius;

                var segment = new RadialMenuSegment(content, segAngle, minAngle, minAngle + stepAngle);
                segment.configure(data[i], isRoot : true);
                segment.content.configure(data[i]);
                segments.Add(segment);
                createChildSegments(segment, data[i].children);
                for (int j = 0; j < data[i].children.Length; j++) {
                    updateChildSegment(segment.children[j], j, 0, segment.angle, segment.children.Count);
                }
            }
            updateRootSegments(lerp);
        }

        private void createChildSegments(RadialMenuSegment segment, RadialMenuSegmentData[] data) {
            for (int i = 0; i < data.Length; i++) {
                RadialMenuSegmentContent component = Instantiate(segmentReference, Vector3.zero, Quaternion.identity, transform);
                Image image = component.background;
                image.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                image.material = new Material(image.material);

                float stepAngle = (angle / this.data.Length) / (float)data.Length;
                float minAngle = segment.angle - (angle / this.data.Length) / 2f + i * stepAngle;
                float textRadius = image.rectTransform.rect.width * childCenterRadius;
                float segmentAngle = minAngle + stepAngle / 2f;

                var child = new RadialMenuSegment(component, segmentAngle, minAngle, minAngle + stepAngle);
                child.configure(data[i], isRoot : false);
                child.content.configure(data[i]);
                segment.children.Add(child);
            }
        }

        private void updateRootSegment(RadialMenuSegment segment, int idx, float lerp) {
            Image image = segment.content.background;
            float range = this.angle / (float)data.Length / 360f;

            image.material.SetFloat("_OuterRadius", 0.7f * lerp);
            image.material.SetFloat("_InnerRadius", (0.7f - segmentRadius) * lerp);
            image.material.SetFloat("_ArcAngle", (-180 + segment.angle) * lerp);
            image.material.SetFloat("_ArcRange", (range * segmentSpace) * lerp);
            image.material.SetFloat("_Frac", 1.0f);

            if (!segment.isHighlighted) {
                image.material.SetColor("_FillColor", Constants.instance.BM_BASE_COLOR);
            }

            // TODO: this works incorrectly but looks okay
            float centerAngle = segment.angle < 180f ? segment.angle + 180 - 180 * (lerp) : segment.angle - 180 + 180 * (lerp);
            float textRadius = image.rectTransform.rect.width * rootCenterRadius;
            Vector3 center = Vector3.zero;

            center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * (centerAngle)) * -1;
            center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * (centerAngle));

            segment.content.animateUpdate(lerp, center);
        }

        private void updateChildSegment(RadialMenuSegment segment, int idx, float lerp, float angle, int neighborsCount) {
            Image image = segment.content.background;
            TMP_Text text = image.GetComponentInChildren<TMP_Text>();
            float range = ((this.angle / (float)data.Length) / (float)neighborsCount) / 360f;

            image.material.SetFloat("_OuterRadius", 1f * lerp);
            image.material.SetFloat("_InnerRadius", (1f - 0.28f) * lerp);
            image.material.SetFloat("_ArcAngle", (-180 + segment.angle));
            image.material.SetFloat("_ArcRange", (range * segmentSpace));
            image.material.SetFloat("_Frac", 1.0f);

            if (!segment.isHighlighted) {
                image.material.SetColor("_FillColor", Constants.instance.BM_BASE_COLOR);
            }

            float textRadius = image.rectTransform.rect.width * childCenterRadius;
            Vector3 center = Vector3.zero;
            center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * (segment.angle)) * -1;
            center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * (segment.angle));

            segment.content.animateUpdate(lerp, center);
        }

        private void clearSegments() {
            foreach (RadialMenuSegment root in segments) {
                foreach (RadialMenuSegment child in root.children) {
                    Destroy(child.content.gameObject);
                }
                Destroy(root.content.gameObject);
            }
            segments.Clear();
        }
    }
}