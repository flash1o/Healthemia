using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UF_Healthemia.Models;
using UF_Healthemia.Models.Diseases;

namespace UF_Healthemia.Providers
{
    internal class HealthemiaDiseasesProvider
    {
        private List<HealthemiaDisease> _diseases;

        internal T GetRandomDiseaseOfType<T>() where T : HealthemiaDisease
        {
            var diseasesOfType = _diseases.Where(x => x is T).ToList();
            return (T)diseasesOfType[UnityEngine.Random.Range(0, diseasesOfType.Count - 1)];
        }
    }
}
