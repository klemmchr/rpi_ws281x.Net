using System;
using System.Drawing;
using ws281x.Net.Model;

namespace ws281x.Net
{
    /// <summary>
    ///     Provides operations for neopixel leds.
    /// </summary>
    public class Neopixel : IDisposable
    {
        private readonly ws2811_channel_t _channel;

        private readonly ws2811_t _leds;
        private bool _disposing;

        /// <summary>
        ///     Initializes a new <see cref="Neopixel" /> instance.
        /// </summary>
        /// <param name="ledCount">The count of leds.</param>
        /// <param name="pin">The GPIO pin index connected to the display signal line.</param>
        /// <param name="frequency">The frequency of the display signal.</param>
        /// <param name="dma">The DMA channel to use.</param>
        /// <param name="invert">Whether the signal line should be inverted</param>
        /// <param name="brightness">The overall brightness.</param>
        /// <param name="channel">The PWM channel to use</param>
        public Neopixel(int ledCount, int pin, uint frequency = 800000, int dma = 10, bool invert = false,
            byte brightness = 255, int channel = 0) : this(ledCount, pin, rpi_ws281x.WS2811_STRIP_RBG, frequency, dma,
            invert, brightness, channel)
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="Neopixel" /> instance.
        /// </summary>
        /// <param name="ledCount">The count of leds.</param>
        /// <param name="pin">The GPIO pin index connected to the display signal line.</param>
        /// <param name="stripType">The strip type to use.</param>
        /// <param name="frequency">The frequency of the display signal.</param>
        /// <param name="dma">The DMA channel to use.</param>
        /// <param name="invert">Whether the signal line should be inverted</param>
        /// <param name="brightness">The overall brightness.</param>
        /// <param name="channel">The PWM channel to use</param>
        public Neopixel(int ledCount, int pin, int stripType, uint frequency = 800000, int dma = 10,
            bool invert = false, byte brightness = 255, int channel = 0)
        {
            _leds = new ws2811_t();

            // Initialize the channels to zero
            for (var i = 0; i < 2; i++)
            {
                var chan = rpi_ws281x.ws2811_channel_get(_leds, i);
                var chanRef = ws2811_channel_t.getCPtr(chan);
                rpi_ws281xPINVOKE.ws2811_channel_t_count_set(chanRef, 0);
                rpi_ws281xPINVOKE.ws2811_channel_t_gpionum_set(chanRef, 0);
                rpi_ws281xPINVOKE.ws2811_channel_t_invert_set(chanRef, 0);
                rpi_ws281xPINVOKE.ws2811_channel_t_brightness_set(chanRef, 0);
            }

            // Initialize the channel in use
            _channel = rpi_ws281x.ws2811_channel_get(_leds, channel);
            var channelRef = ws2811_channel_t.getCPtr(_channel);
            rpi_ws281xPINVOKE.ws2811_channel_t_count_set(channelRef, ledCount);
            rpi_ws281xPINVOKE.ws2811_channel_t_gpionum_set(channelRef, pin);
            rpi_ws281xPINVOKE.ws2811_channel_t_invert_set(channelRef, Convert.ToInt32(invert));
            rpi_ws281xPINVOKE.ws2811_channel_t_brightness_set(channelRef, brightness);
            rpi_ws281xPINVOKE.ws2811_channel_t_strip_type_set(channelRef, stripType);

            // Initialize the controller
            var ledRef = ws2811_t.getCPtr(_leds);
            rpi_ws281xPINVOKE.ws2811_t_freq_set(ledRef, frequency);
            rpi_ws281xPINVOKE.ws2811_t_dmanum_set(ledRef, dma);

            //Grab the led data array.
            LedList = new LedList(_channel);
        }

        /// <summary>
        ///     Gets the LedList.
        /// </summary>
        public LedList LedList { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposing)
                return;

            _disposing = true;
            if (_leds != null)
                rpi_ws281xPINVOKE.delete_ws2811_t(ws2811_t.getCPtr(_leds));

            _leds?.Dispose();
            LedList?.Dispose();
        }

        /// <summary>
        ///     Initializes the library. Must be called before other functions are called.
        /// </summary>
        public void Begin()
        {
            var res = rpi_ws281x.ws2811_init(_leds);
            if (res != ws2811_return_t.WS2811_SUCCESS)
                throw new NeopixelException($"ws2811_init failed with code {res} ({GetReturnString(res)}");
        }

        /// <summary>
        ///     Updates the display with the data from the LED buffer.
        /// </summary>
        public void Show()
        {
            var res = rpi_ws281x.ws2811_render(_leds);
            if (res != ws2811_return_t.WS2811_SUCCESS)
                throw new NeopixelException($"ws2811_render failed with code {res} ({GetReturnString(res)}");
        }

        /// <summary>
        ///     Sets LED at position ledIndex to the provided color.
        /// </summary>
        /// <param name="ledIndex">The index of the LED.</param>
        /// <param name="color">The color to set.</param>
        public void SetPixelColor(int ledIndex, Color color)
        {
            LedList.SetColor(ledIndex, color);
        }

        /// <summary>
        ///     Scales the brightness of each LED in the buffer.
        /// </summary>
        /// <param name="brightness">The brightness to set (0 is the darkest and 255 is the brightest)</param>
        public void SetBrightness(byte brightness)
        {
            if (brightness > 255) throw new ArgumentOutOfRangeException(nameof(brightness));
            rpi_ws281xPINVOKE.ws2811_channel_t_brightness_set(ws2811_channel_t.getCPtr(_channel), brightness);
        }

        /// <summary>
        ///     Gets the brightness value for each LED in the buffer.
        /// </summary>
        /// <returns>The brightness between 0 (the darkest) and 255 (the brightest).</returns>
        public byte GetBrightness()
        {
            return rpi_ws281xPINVOKE.ws2811_channel_t_brightness_get(ws2811_channel_t.getCPtr(_channel));
        }

        /// <summary>
        ///     Gets the number of pixels in the display.
        /// </summary>
        /// <returns>The number of pixels in the display.</returns>
        public int GetNumberOfPixels()
        {
            return rpi_ws281xPINVOKE.ws2811_channel_t_count_get(ws2811_channel_t.getCPtr(_channel));
        }

        private string GetReturnString(ws2811_return_t ret)
        {
            return rpi_ws281x.ws2811_get_return_t_str(ret);
        }
    }
}