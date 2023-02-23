using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UF_Healthemia.Models.Medication
{
    internal class DiseaseMedication 
    {
        public ushort ItemId { get; set; }
        public Type DiseaseType { get; set; }
        public float MedicationPoints { get; set; }

        public DiseaseMedication(ushort itemId, HealthemiaDisease diseaseType, float medicationPoints)
        {
            ItemId = itemId;
            DiseaseType = diseaseType.GetType();
            MedicationPoints = medicationPoints;
        }
        
    }
}
