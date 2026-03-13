using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Length-prefixed JSON 직렬화.
    /// 와이어 형식: [4-byte Little-Endian 길이][UTF-8 JSON body]
    /// JsonLengthPrefixSerializer (OPC UA IPC) 패턴과 동일.
    /// </summary>
    public sealed class PythonAiSerializer
    {
        private const int MAX_MESSAGE_SIZE = 10_000_000; // 10MB

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.None
        };

        public async Task WriteAsync(Stream stream, PythonAiMessage msg)
        {
            var json = JsonConvert.SerializeObject(msg, Settings);
            var body = Encoding.UTF8.GetBytes(json);
            var len = BitConverter.GetBytes(body.Length); // Little-Endian on x86

            await stream.WriteAsync(len, 0, 4).ConfigureAwait(false);
            await stream.WriteAsync(body, 0, body.Length).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }

        public async Task<PythonAiMessage> ReadAsync(Stream stream)
        {
            var lenBuf = await ReadExactAsync(stream, 4).ConfigureAwait(false);
            int len = BitConverter.ToInt32(lenBuf, 0);

            if (len <= 0 || len > MAX_MESSAGE_SIZE)
                throw new InvalidDataException(String.Format("Invalid message length: {0}", len));

            var body = await ReadExactAsync(stream, len).ConfigureAwait(false);
            var json = Encoding.UTF8.GetString(body);

            try
            {
                return JsonConvert.DeserializeObject<PythonAiMessage>(json, Settings);
            }
            catch (Exception ex)
            {
                var preview = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
                throw new InvalidDataException(
                    String.Format("JSON deserialization failed: {0}\nJSON: {1}", ex.Message, preview), ex);
            }
        }

        private static async Task<byte[]> ReadExactAsync(Stream s, int size)
        {
            var buf = new byte[size];
            int read = 0;

            while (read < size)
            {
                int r = await s.ReadAsync(buf, read, size - read).ConfigureAwait(false);
                if (r == 0)
                    throw new EndOfStreamException();
                read += r;
            }

            return buf;
        }
    }
}
