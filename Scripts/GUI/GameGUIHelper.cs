using UnityEngine;
using System.Collections;

public class GameGUIHelper : MonoBehaviour {
    //LINE_WIDTH defines the board width of the selection rectangle
    private const float LINE_WIDTH = 2;

    public static Color color = Color.gray;

    private static Texture2D s_Texture = null;

    public enum RectPosition 
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3,
        Center = 4
    }

    public static void DrawDot(Vector2 pos, Texture2D texture)
    {
        float screenGUIX = pos.x;
        float screenGUIY = Screen.height - pos.y;

        GUI.DrawTexture(new Rect(screenGUIX, screenGUIY, 10, 10), texture);
    }

    /// <summary>
    /// Draw a GUI horiz line on screen.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public static void DrawHorizontalLine(float leftX, float leftY, float length)
    {
        if (s_Texture == null)
        {
          s_Texture = new Texture2D(1, 1);
          s_Texture.SetPixel(0, 0, color);
          s_Texture.Apply();
        }
        GUI.DrawTexture(new Rect(leftX, leftY, length, LINE_WIDTH), s_Texture);
    }

    public static void DrawVerticalLine(float topX, float topY, float length)
    {
        if (s_Texture == null)
        {
          s_Texture = new Texture2D(1, 1);
          s_Texture.SetPixel(0, 0, color);
          s_Texture.Apply();
        }
        GUI.DrawTexture(new Rect(topX, topY, LINE_WIDTH, length), s_Texture);
    }

    public static Vector2 ConvertScreenTouchCoordToGUICoord(Vector2 screenPos)
    {
        return new Vector2(screenPos.x, Screen.height - screenPos.y);
    }

    public static Vector2 ConvertGUICoordToScreenTouchCoord(Vector2 guiPos)
    {
        return new Vector2(guiPos.x, Screen.height - guiPos.y);
    }

    public static Rect GetSquareOnGUICoordinate(RectPosition position, float edgeLength, Vector2 offset)
    {
        Rect rect;
        switch (position)
        {
            case RectPosition.BottomLeft:
                rect = new Rect(0 + offset.x, Screen.height - offset.y - edgeLength, edgeLength, edgeLength); //Left bottom button
                break;
            case RectPosition.BottomRight:
                rect = new Rect(Screen.width + offset.x - edgeLength, Screen.height + offset.y - edgeLength, edgeLength, edgeLength);
                break;
            case RectPosition.TopLeft:
                rect = new Rect(0 + offset.x, 0 + offset.y, edgeLength, edgeLength);
                break;
            case RectPosition.TopRight:
                rect = new Rect(Screen.width + offset.x - edgeLength, 0 + offset.y, edgeLength, edgeLength);
                break;
            case RectPosition.Center:
            default:
                rect = new Rect(Screen.width / 2 - edgeLength / 2 + offset.x, Screen.height / 2 - edgeLength / 2 + offset.y, edgeLength, edgeLength);
                break;
        }
        return rect;
    }

    public static Rect GetSquareOnGUICoordinate(RectPosition position, float edgeLength, float offset)
    {
        return GetSquareOnGUICoordinate(position, edgeLength, new Vector2(offset, offset));
        //Rect rect;
        //switch (position)
        //{
        //    case RectPosition.BottomLeft:
        //        rect = new Rect(0 + offset, Screen.height - offset - edgeLength, edgeLength, edgeLength); //Left bottom button
        //        break;
        //    case RectPosition.BottomRight:
        //        rect = new Rect(Screen.width - offset - edgeLength, Screen.height - offset - edgeLength, edgeLength, edgeLength);
        //        break;
        //    case RectPosition.TopLeft:
        //        rect = new Rect(0 + offset, 0 + offset, edgeLength, edgeLength);
        //        break;
        //    case RectPosition.TopRight:
        //        rect = new Rect(Screen.width - offset - edgeLength, 0 + offset, edgeLength, edgeLength);
        //        break;
        //    case RectPosition.Center:
        //    default:
        //        rect = new Rect(Screen.width / 2 - edgeLength / 2, Screen.height / 2 - edgeLength / 2, edgeLength, edgeLength);
        //        break;
        //}
        //return rect;
    }

    /// <summary>
    /// Default offset = 10
    /// </summary>
    /// <param name="position"></param>
    /// <param name="edgeLength"></param>
    /// <returns></returns>
    public static Rect GetSquareOnGUICoordinate(RectPosition position, float edgeLength)
    {
        float offset = 10f;
        return GetSquareOnGUICoordinate(position, edgeLength, offset);
    }
}
