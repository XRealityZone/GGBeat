
using System.Collections;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.LowLevel;
using System;
using UnityEngine.InputSystem.EnhancedTouch;

namespace GGBeat
{
    public class SplashManager : MonoBehaviour
    {
        public float splashTime = 2f;

        public GameObject? splashObject;

        public GameObject? birdObject;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetupTransition();
            EnhancedTouchSupport.Enable();
        }

        private void SetupTransition()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            Debug.Log("SplashManager Start: " + AppManager.Instance.isSplashOpened);
            if (AppManager.Instance.isSplashOpened)
            {
                birdObject?.SetActive(true);
                splashObject?.SetActive(false);
                CheckBirdAnimation();
            }
            else
            {
                AppManager.Instance.isSplashOpened = true;
                birdObject?.SetActive(false);
                // fade in splash object
                if (splashObject != null)
                {
                    StartCoroutine(WaitAndDo(0, () => audioSource.Play()));
                    StartCoroutine(FadeIn(splashObject, splashTime / 2, 0));
                    StartCoroutine(FadeOutAndDestroy(splashObject, splashTime / 2, splashTime / 2));
                }
                if (birdObject != null)
                {
                    // active bird object after splash object is destroyed
                    StartCoroutine(WaitAndDo(splashTime, () => birdObject.SetActive(true)));
                    StartCoroutine(FadeIn(birdObject, splashTime / 2, splashTime));
                }
            }
        }

        void OnEnable()
        {
            // TODO: ready to use touch input
        }

        void Update()
        {
            CheckBirdTap();
        }

        private void CheckBirdTap()
        {
            var activeTouches = Touch.activeTouches;

            if (activeTouches.Count > 0)
            {
                var primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);
                if (activeTouches[0].phase == TouchPhase.Began)
                {
                    // allow balloons to be popped with a poke or indirect pinch
                    if (primaryTouchData.Kind == SpatialPointerKind.IndirectPinch)
                    {

                        Debug.Log("Primary touch data: " + primaryTouchData.Kind);
                        var targetObject = primaryTouchData.targetObject;
                        if (targetObject != null)
                        {
                            if (targetObject == birdObject)
                            {
                                if (AppManager.Instance.isMainMenuOpen)
                                {
                                    AppManager.Instance.CloseMainMenu();
                                }
                                else
                                {
                                    AppManager.Instance.OpenMainMenu();
                                }
                                CheckBirdAnimation();
                            }
                        }
                    }
                }
            }
        }

        void OpenMainMenuAfterTwoSeconds()
        {
            StartCoroutine(WaitAndDo(splashTime + 2, () => AppManager.Instance.OpenMainMenu()));
        }

        private void CheckBirdAnimation()
        {
            Animator animator = birdObject.GetComponent<Animator>();
            Debug.Log("CheckBirdAnimation: " + AppManager.Instance.isMainMenuOpen);
            if (AppManager.Instance.isMainMenuOpen)
            {
                animator.enabled = false;
            }
            else
            {
                animator.enabled = true;
            }
        }
        IEnumerator FadeIn(GameObject obj, float duration, float delay)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = obj.AddComponent<CanvasGroup>();
            }

            float rate = 1.0f / duration;
            float progress = 0.0f;

            while (progress < 1.0f)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            canvasGroup.alpha = 1;
        }

        IEnumerator FadeOutAndDestroy(GameObject obj, float duration, float delay)
        {
            yield return new WaitForSeconds(delay);

            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = obj.AddComponent<CanvasGroup>();
            }

            float rate = 1.0f / duration;
            float progress = 0.0f;

            while (progress < 1.0f)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            canvasGroup.alpha = 0;
            Destroy(obj);
        }

        IEnumerator WaitAndDo(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}
