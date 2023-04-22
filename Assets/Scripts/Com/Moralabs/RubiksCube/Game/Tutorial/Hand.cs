using Com.Moralabs.RubiksCube.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.Tutorial {
    public class Hand : MonoBehaviour {

      
        [SerializeField]
        private float duration = 2;

        private Vector3 firstPos, finalPos;
        private float time;


        private void Update() {
            PositionTransform();
        }

        private void PositionTransform() {
            if (time < duration) {
                time += Time.deltaTime;

                if (time >= duration) {
                    time = 0;
                }
                ((RectTransform)transform).anchoredPosition = Vector3.Lerp(firstPos, finalPos, time / duration);
            }
        }

        public void SetHand(Vector3 start, Vector3 end) {
            firstPos = start;
            finalPos = end;
            time = 0;
            ((RectTransform)transform).anchoredPosition = start;
            gameObject.SetActive(true);
        }

        public void StopHand() {
            gameObject.SetActive(false);
        }


    }
}