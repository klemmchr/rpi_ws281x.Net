using System.Drawing;

namespace ws281x.Net.Model
{
    /// <summary>
    ///     Provides extensions for the <see cref="Color" /> struct.
    /// </summary>
    internal static class ColorExtensions
    {
        /// <summary>
        ///     Gets the 24 bit RGB representation of the color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>The 24 bit RGB representation.</returns>
        public static uint ToRgb(this Color color)
        {
            return ((uint) color.A << 24) | ((uint) color.R << 16) | ((uint) color.G << 8) | color.B;
        }
    }
}