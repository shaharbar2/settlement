using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
    [SerializeField] private Animator animatorActiveIn;
    [SerializeField] private Animator animatorActiveOut;
    [SerializeField] private Animator animatorClosed;
    [SerializeField] private Animator animatorOpening;
    [SerializeField] private Animator animatorClosing;

    private int idx = 0;
    private float elapsed = 0;
    private Dictionary < int, (Animator, float) > timings;

    void Awake() {
        timings = new Dictionary < int, (Animator, float) > {
            [0] = (animatorClosed, 2f),
            [1] = (animatorOpening, 1.7f),
            [2] = (animatorActiveIn, 2.06f),
            [3] = (animatorActiveOut, 2.06f),
            [4] = (animatorClosing, 1.03f),
            [5] = (null, 1.03f),
        };
        foreach (var timing in timings) {
            timing.Value.Item1?.gameObject.SetActive(false);
        }
    }

    void Start() {
        timings[idx].Item1.gameObject.SetActive(true);
        timings[idx].Item1.Play("Entry");
    }

    void Update() {
        Animator animator = timings[idx].Item1;
        float time = timings[idx].Item2;

        elapsed += Time.deltaTime;
        if (elapsed > time) {
            elapsed = 0;
            animator?.gameObject.SetActive(false);
            idx++;
            if (idx >= timings.Count) {
                idx = 0;
            }
            animator = timings[idx].Item1;
            animator?.gameObject.SetActive(true);
            animator?.Play("Entry");
        }
    }
}