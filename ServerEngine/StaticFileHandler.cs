using System.Net.Sockets;

namespace ServerEngine
{
    public class StaticFileHandler : IHandler
    {
        private readonly string _path;

        public StaticFileHandler(string path)
        {
            _path = path;
        }

        public void Handle(Stream networkStream, Request request)
        {
            using (var writer = new StreamWriter(networkStream))
            {
                var filePath = Path.Combine(_path, request.Path.Substring(1));

                if (File.Exists(filePath))
                {
                    ResponseWriter.WriteStatus(System.Net.HttpStatusCode.OK, networkStream);

                    using (var fileStream = File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(networkStream);
                    }
                }
                else
                {
                    ResponseWriter.WriteStatus(System.Net.HttpStatusCode.NotFound, networkStream);
                }

                Console.WriteLine(filePath);
            }
        }

        public async Task HandleAsync(Stream networkStream, Request request)
        {
            using (var writer = new StreamWriter(networkStream))
            {
                var filePath = Path.Combine(_path, request.Path.Substring(1));

                if (File.Exists(filePath))
                {
                    await ResponseWriter.WriteStatusAsync(System.Net.HttpStatusCode.OK, networkStream);

                    using (var fileStream = File.OpenRead(filePath))
                    {
                        await fileStream.CopyToAsync(networkStream);
                    }
                }
                else
                {
                    await ResponseWriter.WriteStatusAsync(System.Net.HttpStatusCode.NotFound, networkStream);
                }

                Console.WriteLine(filePath);
            }
        }
    }
}