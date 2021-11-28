using System.Collections;
using System.Collections.Generic;
using Logic;
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
        
        public static Vector3 ConvertHexVet3ToVec3(Vector3 vector3)
        {
            var defaultOffsetX = Managers.Data.ConstantsTableData.DefaultHexPositionOffsetX;
            var defaultOffsetY = Managers.Data.ConstantsTableData.DefaultHexPositionOffsetY;
            return new Vector3(vector3.x / defaultOffsetX, vector3.y / defaultOffsetY, 0);
        }

        public static GameViewSubSystem GetGameViewSubsystem() => Managers.Game.GameViewSubSystem;
        public static GameModelSubSystem GetGameModelSubsystem() => Managers.Game.GameModelSubSystem;
        
        public static Direction GetSwapDirection(Vector2Int beforeBlockPos, Vector2Int afterBlockPos)
        {
            Direction resultDirection = default;
            if (beforeBlockPos.x == afterBlockPos.x)
            {
                resultDirection = beforeBlockPos.y < afterBlockPos.y ? Direction.Top : Direction.Bottom;
            }
            else if (beforeBlockPos.y < afterBlockPos.y)
            {
                resultDirection = beforeBlockPos.x < afterBlockPos.x ? Direction.TopRight : Direction.TopLeft;
            }
            else if (beforeBlockPos.y > afterBlockPos.y)
            {
                resultDirection = beforeBlockPos.x < afterBlockPos.x ? Direction.BottomRight : Direction.BottomLeft;
            }
            return resultDirection;
        }
    }
}

