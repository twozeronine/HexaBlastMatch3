using System;

namespace Logic
{
    public enum Scene
    {
        Unknown,
        Game,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum TileType
    {
        Empty,
        Normal,
    }

    public enum TileColor
    {
        Gray,
    }

    public enum StageMissionType
    {
        Top,
    }

    public enum Direction
    {
        None,
        Top,
        TopLeft,
        TopRight,
        Bottom,
        BottomLeft,
        BottomRight,
    }
    public enum BlockColor
    {
        Red,
        Yellow,
        Green,
        Blue,
        Purple
    }

    public enum BlockType
    {
        Empty,
        Normal,
        Top,
    }
}