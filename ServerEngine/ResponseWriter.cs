using System.Net;

namespace ServerEngine
{
    /// <summary>
    /// Генератор ответов
    /// </summary>
    internal static class ResponseWriter
    { 
        /// <summary>
        /// Записать статус
        /// </summary>
        /// <param name="code">Код ответа</param>
        /// <param name="stream">Поток запроса</param>
        public static void WriteStatus(HttpStatusCode code, Stream stream)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            writer.WriteLine($"HTTP/1.0 {(int)code} {code}");
            writer.WriteLine();
        }

        /// <summary>
        /// Записать статус (асинхронно)
        /// </summary>
        /// <param name="code">Код ответа</param>
        /// <param name="stream">Поток запроса</param>
        public async static Task WriteStatusAsync(HttpStatusCode code, Stream stream)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            await writer.WriteLineAsync($"HTTP/1.0 {(int)code} {code}");
            await writer.WriteLineAsync();
        }
    }
}