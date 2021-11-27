using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class GameUtil : MonoBehaviour
    {
        public static Vector3 ConvertVec3ToHexVet3(Vector3 vector3)
        {
            var defaultOffsetX = Managers.Data.ConstantsTableData.DefaultHexPositionOffsetX;
            var defaultOffsetY = Managers.Data.ConstantsTableData.DefaultHexPositionOffsetY;
            var calEvenOffsetY = vector3.x % 2 == 0 ? defaultOffsetY * 2 : 0; 
            return new Vector3(vector3.x * defaultOffsetX, vector3.y * defaultOffsetY + calEvenOffsetY , 0);
        }
    }
}

