using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class UtilsFPS : MonoBehaviour
    {
        [Range(0f, 120f)]
        [SerializeField] private int _targetFPS;

        private void OnValidate()
        {
            Application.targetFrameRate = _targetFPS;
        }
    }
}
