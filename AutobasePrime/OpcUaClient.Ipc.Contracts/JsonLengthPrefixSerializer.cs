using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    public sealed class JsonLengthPrefixSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.None,
            Converters = { new IpcMessageConverter() },
            Error = (sender, args) =>
            {
                // 역직렬화 오류 무시 (복잡한 OPC UA 타입 처리)
                args.ErrorContext.Handled = true;
            }
        };

        public void Write(Stream stream, IpcMessage msg)
        {
            var json = JsonConvert.SerializeObject(msg, Settings);
            var body = Encoding.UTF8.GetBytes(json);
            var len = BitConverter.GetBytes(body.Length);

            stream.Write(len, 0, 4);
            stream.Write(body, 0, body.Length);
            stream.Flush();
        }

        public async Task WriteAsync(Stream stream, IpcMessage msg)
        {
            var json = JsonConvert.SerializeObject(msg, Settings);
            var body = Encoding.UTF8.GetBytes(json);
            var len = BitConverter.GetBytes(body.Length);

            await stream.WriteAsync(len, 0, 4).ConfigureAwait(false);
            await stream.WriteAsync(body, 0, body.Length).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }

        public IpcMessage Read(Stream stream)
        {
            var lenBuf = ReadExact(stream, 4);
            int len = BitConverter.ToInt32(lenBuf, 0);

            if (len <= 0 || len > 10_000_000)
                throw new InvalidDataException($"Invalid IPC length: {len}");

            var body = ReadExact(stream, len);
            var json = Encoding.UTF8.GetString(body);

            try
            {
                return JsonConvert.DeserializeObject<IpcMessage>(json, Settings);
            }
            catch (Exception ex)
            {
                // 디버깅: JSON 파싱 실패 시 원본 출력
                var preview = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
                throw new InvalidDataException(
                    $"JSON deserialization failed: {ex.Message}\nJSON preview: {preview}", ex);
            }
        }

        public async Task<IpcMessage> ReadAsync(Stream stream)
        {
            var lenBuf = await ReadExactAsync(stream, 4).ConfigureAwait(false);
            int len = BitConverter.ToInt32(lenBuf, 0);

            if (len <= 0 || len > 10_000_000)
                throw new InvalidDataException($"Invalid IPC length: {len}");

            var body = await ReadExactAsync(stream, len).ConfigureAwait(false);
            var json = Encoding.UTF8.GetString(body);

            try
            {
                return JsonConvert.DeserializeObject<IpcMessage>(json, Settings);
            }
            catch (Exception ex)
            {
                // 디버깅: JSON 파싱 실패 시 원본 출력
                var preview = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
                throw new InvalidDataException(
                    $"JSON deserialization failed: {ex.Message}\nJSON preview: {preview}", ex);
            }
        }

        private static byte[] ReadExact(Stream s, int size)
        {
            var buf = new byte[size];
            int read = 0;

            while (read < size)
            {
                int r = s.Read(buf, read, size - read);
                if (r == 0)
                    throw new EndOfStreamException();
                read += r;
            }

            return buf;
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
