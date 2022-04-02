using System.Drawing;

namespace Checkers
{
    /// <summary>
    /// Graphics Extensio functions written by me for use in the project 
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Draw Circle 
        /// </summary>
        /// <param name="g"> Graphics object </param>
        /// <param name="pen"> Pen to draw with </param>
        /// <param name="centerX"> X Coordinate of circle center </param>
        /// <param name="centerY"> Y Coordinate of circle center </param>
        /// <param name="radius"> Radius of circle </param>
        public static void DrawCircle(this Graphics g, Pen pen,
                                      int centerX, int centerY, int radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        /// <summary>
        /// Fill Circle 
        /// </summary>
        /// <param name="g"> Graphics object </param>
        /// <param name="brush"> Brush to Fill circle with </param>
        /// <param name="centerX"> X Coordinate of circle center </param>
        /// <param name="centerY"> Y Coordinate of circle center </param>
        /// <param name="radius"> Radius of circle </param>
        public static void FillCircle(this Graphics g, Brush brush,
                                      int centerX, int centerY, int radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
    }
}
