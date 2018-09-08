using System;
using System.Runtime.Serialization;

namespace ws281x.Net.Model
{
    /// <inheritdoc />
    public class NeopixelException : Exception
    {
        /// <inheritdoc />
        public NeopixelException()
        {
        }

        /// <inheritdoc />
        public NeopixelException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public NeopixelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected NeopixelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}