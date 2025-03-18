#nullable enable

using System;

namespace DebugToolkit
{
    internal ref struct ValueStringBuilder
    {
        private readonly Span<char> _buffer;
        private int _written;

        public ValueStringBuilder(in Span<char> buffer)
        {
            _buffer = buffer;
            _written = 0;
        }

        public void Append(in ReadOnlySpan<char> value)
        {
            value.CopyTo(_buffer.Slice(_written));
            _written += value.Length;
        }

        public void Append(in double value, in ReadOnlySpan<char> format)
        {
            value.TryFormat(_buffer.Slice(_written), out var written, format);
            _written += written;
        }

        public void Append(in float value, in ReadOnlySpan<char> format)
        {
            value.TryFormat(_buffer.Slice(_written), out var written, format);
            _written += written;
        }

        public void AppendLine()
        {
            var line = Environment.NewLine.AsSpan();
            line.CopyTo(_buffer.Slice(_written));
            _written += line.Length;
        }

        public override string ToString() => new string(_buffer.Slice(0, _written));
    }
}