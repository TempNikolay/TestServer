namespace ServerEngine
{
    /// <summary>
    /// Обработчик запросов
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Обработать запрос
        /// </summary>
        /// <param name="stream">Поток с запросом</param>
        /// <param name="request">Информация о запросе</param>
        void Handle(Stream stream, Request request);

        /// <summary>
        /// Обработать запрос (асинхронно)
        /// </summary>
        /// <param name="stream">Поток с запросом</param>
        /// <param name="request">Информация о запросе</param>
        Task HandleAsync(Stream stream, Request request);
    }
}