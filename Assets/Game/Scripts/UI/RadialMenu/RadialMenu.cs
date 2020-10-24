using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class RadialMenuSegmentData {
    public RadialMenu layer;
    public Image image;
    public float angle;
    public float minAngle;
    public float maxAngle;
    public Vector3 center;

    public RadialMenuSegmentData(Image image, RadialMenu layer, float angle, float minAngle, float maxAngle, Vector3 center) {
        this.layer = layer;
        this.image = image;
        this.angle = angle;
        this.minAngle = minAngle;
        this.maxAngle = maxAngle;
        this.center = center;
    }
}

public class RadialMenu : MonoBehaviour {
    [SerializeField] private int segmentsCount;
    [SerializeField] private float segmentRadius;
    [SerializeField] private float segmentSpace;
    [SerializeField] private Image segmentReference;

    private List<RadialMenuSegmentData> segments = new List<RadialMenuSegmentData>();
    private LTDescr showTween;
    private LTDescr hideTween;

    private float angle = 360;
    private float lerp = 0;

    void Awake() {
        segmentReference.gameObject.SetActive(false);
        createSegments();
    }

    void Update() {

    }

    /// Public -- 

    public void showAnimated() {
        if (hideTween != null) {
            LeanTween.cancel(hideTween.id);
            hideTween = null;
        } 
        if (showTween == null) {
            float time = Constants.instance.BUILD_MENU_SHOW_DURATION * (1f - lerp);
            LeanTweenType ease = Constants.instance.BUILD_MENU_SHOW_EASE;
            showTween = LeanTween.value(gameObject, lerp, 1f, time).setEase(ease).setOnUpdate((float value) => { 
                this.lerp = value;
                updateAllSegments(lerp: value);
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
            float time = Constants.instance.BUILD_MENU_HIDE_DURATION * lerp;
            LeanTweenType ease = Constants.instance.BUILD_MENU_HIDE_EASE;
            hideTween = LeanTween.value(gameObject, lerp, 0, time).setEase(ease).setOnUpdate((float value) => { 
                this.lerp = value;
                updateAllSegments(lerp: value);
            }).setOnComplete(() => {
                hideTween = null;
            });
        }
    }

    /// Private -- 

    private void updateAllSegments(float lerp) {
        for (int i = 0; i < segments.Count; i++) {
            updateSegment(idx: i, lerp: lerp);
        }
    }

    private void redrawArcs() {
        clearSegments();
        createSegments();
    }

    private void createSegments() {
        for (int i = 0; i < segmentsCount; i++) {
            Image image = Instantiate(segmentReference, Vector3.zero, Quaternion.identity, transform);
            image.gameObject.SetActive(true);
            image.transform.localScale = Vector3.one;

            float stepAngle = angle / segmentsCount;
            float segAngle = i * stepAngle;
            float textRadius = image.rectTransform.rect.width * 0.3f;
            Vector3 center = Vector3.zero;
            center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * (segAngle + stepAngle / 2f)) * -1;
            center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * (segAngle + stepAngle / 2f));
            segments.Add(new RadialMenuSegmentData(image, this, segAngle + stepAngle / 2, segAngle, segAngle + stepAngle, center));
        }

        updateAllSegments(lerp);
    }

    private void updateSegment(int idx, float lerp) {
        RadialMenuSegmentData segment = segments[idx];
        Image segmentImage = segment.image;
        segmentImage.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        TMP_Text text = segmentImage.GetComponentInChildren<TMP_Text>();
        Material material = new Material(segmentImage.material);
        float range = this.angle / (float)segmentsCount / 360f;

        material.SetFloat("_OuterRadius", 0.9f * lerp);
        material.SetFloat("_InnerRadius", (0.9f - segmentRadius) * lerp);
        material.SetFloat("_ArcAngle", (-180 + segment.angle) * lerp);
        material.SetFloat("_ArcRange", (range * segmentSpace) * lerp);
        material.SetFloat("_Frac", 1.0f);
        segmentImage.material = new Material(material);

        float textRadius = segmentImage.rectTransform.rect.width * 0.3f;
        Vector3 center = Vector3.zero;
        // TODO: this works incorrectly but looks okay
        float centerAngle = segment.angle < 180f ? segment.angle + 180 - 180 * (lerp) : segment.angle - 180 + 180 * (lerp);

        center.x = textRadius * Mathf.Sin(Mathf.Deg2Rad * (centerAngle)) * -1;
        center.y = textRadius * Mathf.Cos(Mathf.Deg2Rad * (centerAngle));
        
        text.rectTransform.anchoredPosition = center;
        text.color = text.color.setAlpha(lerp);
        text.text = "Menu " + idx;
    }

    private void clearSegments() {
        foreach (RadialMenuSegmentData o in segments) {
            Destroy(o.image.gameObject);
        }

        segments.Clear();
    }

    private RadialMenuSegmentData getSegmentAtAngle(float angle) {
        foreach (RadialMenuSegmentData arc in segments) {
            if (angle <= arc.maxAngle && angle >= arc.minAngle) {
                return arc;
            }
        }
        return null;
    }
}