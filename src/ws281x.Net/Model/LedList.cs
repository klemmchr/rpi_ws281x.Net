using System;
using System.Drawing;

namespace ws281x.Net.Model
{
    /// <summary>
    ///     Represents a list allowing access to all leds of a channel.
    /// </summary>
    public class LedList: IDisposable
    {
        private bool _disposing;
        private readonly ws2811_channel_t _channel;

        /// <summary>
        ///     Initializes a new instance <see cref="LedList" /> instance.
        /// </summary>
        /// <param name="channel"></param>
        public LedList(ws2811_channel_t channel)
        {
            _channel = channel;
        }

        /// <summary>
        ///     Gets the color at the specified position.
        /// </summary>
        /// <param name="pos">The position in the list.</param>
        /// <returns>The color of the LED at the index.</returns>
        public Color GetColor(int pos)
        {
            return Color.FromArgb((int) rpi_ws281x.ws2811_led_get(_channel, pos));
        }

        /// <summary>
        ///     Sets the LED color at the specified position.
        /// </summary>
        /// <param name="pos">The position in the list.</param>
        /// <param name="color">The color to set</param>
        public void SetColor(int pos, Color color)
        {
            rpi_ws281x.ws2811_led_set(_channel, pos, color.ToRgb());
        }
        
        /// <inheritdoc />
        public void Dispose()
        {
            if(_disposing)
                return;

            _disposing = true;
            _channel?.Dispose();
        }
    }
}