using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SyncUtil.Example
{

    [RequireComponent(typeof(GPUFluid), typeof(GPUFluidMouse))]
    public class GPUFluidUpdator : MonoBehaviour
    {

        GPUFluid _fluid;
        GPUFluidMouse _mouse;

        void Start()
        {
            _fluid = GetComponent<GPUFluid>();
            _mouse = GetComponent<GPUFluidMouse>();
        }

        void Update()
        {

            _fluid.Step(CreateStepData(_mouse));
        }

        public static GPUFluid.StepData CreateStepData(GPUFluidMouse mouse)
        {
            return new GPUFluid.StepData()
            {
                deltaTime = Time.deltaTime,
                isAddDataValid = mouse.isDataValid,
                addPos = mouse._pos,
                addRadius = mouse._radius,
                addVelocity = mouse._velocity,
                addColor = mouse._color
            };
        }
    }
}