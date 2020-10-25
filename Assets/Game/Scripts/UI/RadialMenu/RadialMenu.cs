using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class RadialMenuSegment {
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

    public Image image;
    public GameObject contentGameObject { get { return image.GetComponentInChildren<TMP_Text>().transform.parent.gameObject; } }
    public TMP_Text contentText { get { return image.GetComponentInChildren<TMP_Text>(); } }
    public RawImage contentIcon { get { return contentGameObject.GetComponentInChildren<RawImage>(); } }

    public RadialMenuSegment(Image image, float angle, float minAngle, float maxAngle) {
        this.image = image;
        this.angle = angle;
        this.minAngle = minAngle;
        this.maxAngle = maxAngle;
        children = new List<RadialMenuSegment>();
    }
}

public class RadialMenu : MonoBehaviour {
    [SerializeField] private RadialMenuSegmentData[] data;
    [SerializeField] private float segmentRadius;
    [SerializeField] private float segmentSpace;
    [SerializeField] private Image segmentReference;

    [SerializeField] private float rootCenterRadius;
    [SerializeField] private float childCenterRadius;

    private List<RadialMenuSegment> segments = new List<RadialMenuSegment>();
    private LTDescr showTween;
    private LTDescr hideTween;

    private float mouseAngle = 0;
    private float mouseDistance = 0;
    private float angle = 360;
    private float lerp = 0;

    public delegate void RadialMenuEvent(RadialMenuSegmentData data);
    public event RadialMenuEvent onClicked;
    public event RadialMenuEvent onHighlighted;

    private RadialMenuSegment selectedRootSegment;
    private RadialMenuSegment selectedChildSegment;
    private RadialMenuSegment clickedSegment;

    public RadialMenuSegmentData currentSelection { get { return selectedChildSegment?.data; } }

    void Awake() {
        segmentReference.gameObject.SetActive(false);
        createSegments();
    }

    void Update() {
        selectedRootSegment = updateMouseSelection(segments);
        if (Input.GetMouseButtonDown(0) && selectedChildSegment != null) {
            onClicked?.Invoke(selectedChildSegment.data);
            clickedSegment = selectedChildSegment;
            hideAnimated();
            GameObject go = selectedChildSegment.image.gameObject;
            LeanTween.scale(go, Vector3.one * 1.8f, 1f).setOnComplete(() => {
                go.transform.localScale = Vector3.one;
            }).setEase(LeanTweenType.easeOutQuad);
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
        if (showTween != null) {
            LeanTween.cancel(showTween.id);
            showTween = null;
        }
        if (hideTween == null) {
            float time = Constants.instance.BM_TIME_HIDE * lerp;
            LeanTweenType ease = Constants.instance.BM_HIDE_EASE;
            hideTween = LeanTween.value(gameObject, lerp, 0, time).setEase(ease).setOnUpdate((float value) => {
                this.lerp = value;
                updateRootSegments(lerp: value);
            }).setOnComplete(() => {
                hideTween = null;
            });
        }
        onClicked?.Invoke(null);
    }

    /// Private -- 

    private RadialMenuSegment updateMouseSelection(List<RadialMenuSegment> segments) {
        if (lerp > 0.8f) {
            mouseAngle = BabyUtils.InvertAngle(BabyUtils.VectorAngle(Input.mousePosition, BabyUtils.screenBoundsPixels.center));
            mouseDistance = Vector2.Distance(BabyUtils.screenBoundsPixels.center, Input.mousePosition);
        } else if (lerp <= 0.8f) {
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
                if (!segment.isHighlighted) {
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
                selectedChildSegment = updateMouseSelection(segment.children);
                if (!segment.isSelected) {
                    segment.isSelected = true;
                    animateSegmentSelected(segment);
                }
            } else {
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
        float time = 0;
        LeanTweenType easing;
        if (segment.isSelected) {
            time = Constants.instance.BM_TIME_CHILD_SHOW;
            easing = Constants.instance.BM_SHOW_EASE;
        } else {
            if (clickedSegment == null) {
                time = Constants.instance.BM_TIME_CHILD_HIDE;
                easing = Constants.instance.BM_SHOW_EASE;
            } else {
                time = Constants.instance.BM_TIME_CLICKED_CHILD_HIDE;
                easing = Constants.instance.BM_CLICKED_CHILD_HIDE_EASE;
            }
        }
        segment.selectionTween = LeanTween.value(segment.image.gameObject, from, to, time).setOnUpdate((float lerp) => {
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
        segment.highlightTween = LeanTween.value(segment.image.gameObject, from, to, time).setOnUpdate((float lerp) => {
            segment.highlightLerp = lerp;
            Color color = Color.Lerp(Constants.instance.BM_BASE_COLOR, Constants.instance.BM_SELECTED_COLOR, lerp);
            segment.image.material.SetColor("_FillColor", color);
            segment.contentIcon.transform.localScale = Vector3.one * (1f + 0.5f * lerp);
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

    private void redrawArcs() {
        clearSegments();
        createSegments();
    }

    private void createSegments() {
        for (int i = 0; i < data.Length; i++) {
            Image image = Instantiate(segmentReference, Vector3.zero, Quaternion.identity, transform);
            image.gameObject.SetActive(true);
            image.transform.localScale = Vector3.one;

            float stepAngle = angle / data.Length;
            float minAngle = i * stepAngle;
            float segAngle = minAngle + stepAngle / 2f;
            float textRadius = image.rectTransform.rect.width * rootCenterRadius;
            Vector3 center = Vector3.zero;
            center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * segAngle) * -1;
            center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * segAngle);
            var segment = new RadialMenuSegment(image, segAngle, minAngle, minAngle + stepAngle);
            segment.data = data[i];
            segment.isRoot = true;
            segment.contentText.text = data[i].name;
            segment.contentIcon.texture = data[i].iconTexture;
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
            Image image = Instantiate(segmentReference, Vector3.zero, Quaternion.identity, transform);
            image.gameObject.SetActive(true);
            image.transform.localScale = Vector3.one;

            float stepAngle = (angle / this.data.Length) / (float)data.Length;
            float minAngle = segment.angle - (angle / this.data.Length) / 2f + i * stepAngle;
            float textRadius = image.rectTransform.rect.width * childCenterRadius;
            float segmentAngle = minAngle + stepAngle / 2f;
            var child = new RadialMenuSegment(image, segmentAngle, minAngle, minAngle + stepAngle);
            child.data = data[i];
            child.isRoot = false;
            child.contentText.text = data[i].name;
            child.contentIcon.texture = data[i].iconTexture;
            segment.children.Add(child);
        }
    }

    private void updateRootSegment(RadialMenuSegment segment, int idx, float lerp) {
        Image segmentImage = segment.image;
        segmentImage.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Material material = new Material(segmentImage.material);
        float range = this.angle / (float)data.Length / 360f;

        material.SetFloat("_OuterRadius", 0.7f * lerp);
        material.SetFloat("_InnerRadius", (0.7f - segmentRadius) * lerp);
        material.SetFloat("_ArcAngle", (-180 + segment.angle) * lerp);
        material.SetFloat("_ArcRange", (range * segmentSpace) * lerp);
        if (!segment.isHighlighted) {
            material.SetColor("_FillColor", Constants.instance.BM_BASE_COLOR);
        }
        material.SetFloat("_Frac", 1.0f);
        segmentImage.material = new Material(material);

        float textRadius = segmentImage.rectTransform.rect.width * rootCenterRadius;
        Vector3 center = Vector3.zero;
        // TODO: this works incorrectly but looks okay
        float centerAngle = segment.angle < 180f ? segment.angle + 180 - 180 * (lerp) : segment.angle - 180 + 180 * (lerp);

        center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * (centerAngle)) * -1;
        center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * (centerAngle));

        segment.contentGameObject.GetComponent<RectTransform>().anchoredPosition = center;
        segment.contentText.color = segment.contentText.color.setAlpha(lerp);
        segment.contentIcon.color = segment.contentIcon.color.setAlpha(lerp);
    }

    private void updateChildSegment(RadialMenuSegment segment, int idx, float lerp, float angle, int neighborsCount) {
        Image segmentImage = segment.image;
        segmentImage.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        TMP_Text text = segmentImage.GetComponentInChildren<TMP_Text>();
        Material material = new Material(segmentImage.material);
        float range = ((this.angle / (float)data.Length) / (float)neighborsCount) / 360f;
        material.SetFloat("_OuterRadius", 1f * lerp);
        material.SetFloat("_InnerRadius", (1f - 0.28f) * lerp);
        material.SetFloat("_ArcAngle", (-180 + segment.angle));
        material.SetFloat("_ArcRange", (range * segmentSpace));
        if (!segment.isHighlighted) {
            material.SetColor("_FillColor", Constants.instance.BM_BASE_COLOR);
        }
        material.SetFloat("_Frac", 1.0f);
        segmentImage.material = new Material(material);

        float textRadius = segmentImage.rectTransform.rect.width * childCenterRadius;
        Vector3 center = Vector3.zero;
        float centerAngle = segment.angle;
        center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * (centerAngle)) * -1;
        center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * (centerAngle));

        segment.contentGameObject.GetComponent<RectTransform>().anchoredPosition = center;
        segment.contentText.color = segment.contentText.color.setAlpha(lerp);
        segment.contentIcon.color = segment.contentIcon.color.setAlpha(lerp);
    }

    private void clearSegments() {
        foreach (RadialMenuSegment root in segments) {
            foreach (RadialMenuSegment child in root.children) {
                Destroy(child.image.gameObject);
            }
            Destroy(root.image.gameObject);
        }

        segments.Clear();
    }

    private RadialMenuSegment getSegmentAtAngle(float angle) {
        foreach (RadialMenuSegment arc in segments) {
            if (angle <= arc.maxAngle && angle >= arc.minAngle) {
                return arc;
            }
        }
        return null;
    }
}