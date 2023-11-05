using System.Net.Sockets;
using System.Net;
using System.Reflection.PortableExecutable;

namespace ServerEngine
{

    /// <summary>
    /// Сервер
    /// </summary>
    public class ServerHost
    {
        /// <summary>
        /// Обработчик
        /// </summary>
        private readonly IHandler _handler;

        /// <summary>
        /// Сервер
        /// </summary>
        /// <param name="handler">Обработчик</param>
        public ServerHost(IHandler handler)
        {
            _handler = handler;
        }

        #region Старая версия обработчика
        ///// <summary>
        ///// Запустить сервер
        ///// </summary>
        //public void Start()
        //{
        //    var listener = new TcpListener(IPAddress.Any, 80);
        //    listener.Start();

        //    while (true)
        //    {
        //        var client = listener.AcceptTcpClient();

        //        using (var stream = client.GetStream())
        //        using (var reader = new StreamReader(stream))
        //        {
        //            var firstLine = reader.ReadLine();
        //            for (string? line = null; line != String.Empty; line = reader.ReadLine());

        //            var request = RequestParser.Parse(firstLine);

        //            _handler.Handle(stream, request);
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// Запустить сервер (асинхронно)
        /// </summary>
        public async Task StartAsync()
        {
            // Прослушиватель TCP-клиентов
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                // Принимаем запрос клиента
                var client = await listener.AcceptTcpClientAsync();
                // Чтобы не занимать один поток и обработать другой запрос
                var _ = HandleClientAsync(client);
            }
        }

        /// <summary>
        /// Обработать клиент (асинхронно)
        /// </summary>
        /// <param name="client">Клиент</param>
        /// <returns></returns>
        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream()) // получаем поток запроса клиента
            using (var reader = new StreamReader(stream)) // Создаем считыватель потока
            {
                // Читаем только первую строку запроса
                var firstLine = await reader.ReadLineAsync(); 
                // Считываем остальное, чтобы закончить запрос
                for (string? line = null; line != String.Empty; line = await reader.ReadLineAsync()) ;

                // Получаем информацию о запросе из первой строки
                var request = RequestParser.Parse(firstLine);

                // Обрабатываем полученный запрос
                await _handler.HandleAsync(stream, request);
            }
        }

        /// <summary>
        /// Обработать клиент
        /// </summary>
        /// <param name="client">Клиент</param>
        private void HandleClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = reader.ReadLine();
                    for (string? line = null; line != String.Empty; line = reader.ReadLine()) ;

                    var request = RequestParser.Parse(firstLine);

                    _handler.Handle(stream, request);
                }
            });
        }
    }
}