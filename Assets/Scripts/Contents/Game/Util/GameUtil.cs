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
            return new Vector3(vector3.x * defaultOffsetX, vector3.y * defaultOffsetY, 0);
        }
    }
}

