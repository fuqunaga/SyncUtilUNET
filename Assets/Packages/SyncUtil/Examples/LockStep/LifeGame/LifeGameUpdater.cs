using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyncUtil.Example
{

    [RequireComponent(typeof(LifeGame))]
    public class LifeGameUpdater : MonoBehaviour
    {
        public float _resolutionScale = 0.5f;
        static int _lastWidth;
        static int _lastHeight;
        LifeGame _lifeGame;

        private void Start()
        {
            _lifeGame = GetComponent<LifeGame>();
        }


        void Update()
        {
            _lifeGame.Step(CreateStepData(_resolutionScale));
        }

        public static LifeGame.StepData CreateStepData(float resolutionScale)
        {
            var isResize = (Screen.width != _lastWidth) || (Screen.height != _lastHeight);
            if (isResize)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
            }

            return new LifeGame.StepData()
            {
                isResize = isResize,
                width = Mathf.FloorToInt(_lastWidth * resolutionScale),
                height = Mathf.FloorToInt(_lastHeight * resolutionScale),
                randSeed = Mathf.FloorToInt(Random.value * int.MaxValue),
                isInputEnable = Input.GetMouseButton(0),
                inputPos = Input.mousePosition * resolutionScale,
                deltaTime = Time.deltaTime
            };

        }
    }
}