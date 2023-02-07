using System;
using UF_Healthemia.Helpers;
using UnityEngine;

namespace UF_Healthemia.Services
{
    public class HealthemiaPlayerHealthService : MonoBehaviour
    {
        public HealthemiaPlayerHealthHelper HealthHelper { get; private set; }
        private void Awake()
        {
            HealthHelper = new HealthemiaPlayerHealthHelper();
        }
    }
}