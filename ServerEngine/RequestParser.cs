namespace ServerEngine
{
    /// <summary>
    /// Запрос
    /// </summary>
    /// <param name="Path">Путь до ресурса</param>
    /// <param name="Method">Тип метода</param>
    public record Request(string Path, HttpMethod Method);

    /// <summary>
    /// Парсер запросов
    /// </summary>
    internal static class RequestParser
    {
        /// <summary>
        /// Парсить
        /// </summary>
        /// <param name="head">Заголовок</param>
        /// <returns></returns>
        public static Request Parse(string head)
        {
            var split = head.Split(' ');
            return new Request(split[1], GetMethod(split[0]));
        }

        /// <summary>
        /// Получить метод запроса
        /// </summary>
        /// <param name="method">Строковое представление метода</param>
        /// <returns></returns>
        private static HttpMethod GetMethod(string method)
        {
            if (method == "GET")
            {
                return HttpMethod.Get;
            }

            return HttpMethod.Post;
        }
    }
}