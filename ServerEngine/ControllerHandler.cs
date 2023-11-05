using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ServerEngine
{
    /// <summary>
    /// Обработчик контроллеров
    /// </summary>
    public class ControllerHandler : IHandler
    {
        private readonly Dictionary<string, Func<object>> Routes;

        /// <summary>
        /// Контроллер
        /// </summary>
        /// <param name="controllersAssembly">Сборка с контроллерами</param>
        public ControllerHandler(Assembly controllersAssembly)
        {
                // Ищем все контроллеры и формируем словарь из путей до методов контллеров и обработчики
                Routes = controllersAssembly.GetTypes()
                                             .Where(x => typeof(IController).IsAssignableFrom(x))
                                             .SelectMany(Controller => Controller.GetMethods().Select(Method => new {
                                                 Controller,
                                                 Method
                                             }))
                                             .ToDictionary(
                                                 key => GetPath(key.Controller, key.Method), 
                                                 value => GetEndpointMethod(value.Controller, value.Method)
                                             );
        }

        /// <summary>
        /// Получить метод
        /// </summary>
        /// <param name="controller">Типо контроллера</param>
        /// <param name="method">Информация о методе</param>
        /// <returns></returns>
        private Func<object> GetEndpointMethod(Type controller, MethodInfo method)
        {
            return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
        }

        /// <summary>
        /// Получить путь до метода контроллера
        /// </summary>
        /// <param name="controller">Тип контроллера</param>
        /// <param name="method">Информация о методе</param>
        /// <returns></returns>
        private string GetPath(Type controller, MethodInfo method)
        {
            var name = controller.Name;

            // Если имя контроллера заканчивается на слово "conrtoller", то обрезаем его
            if (name.ToLower().EndsWith("controller", StringComparison.InvariantCulture))
            {
                name = name.Substring(0, name.Length - "controller".Length);
            }

            // Если методом является базовый "Index", то возвращается имя контроллера 
            if (method.Name.Equals("Index", StringComparison.InvariantCulture))
            {
                return "/" + name;
            }

            return "/" + name + "/" + method.Name;
        }

        /// <summary>
        /// Обработать
        /// </summary>
        /// <param name="stream">Поток запроса</param>
        /// <param name="request">Информация о запросе</param>
        public void Handle(Stream stream, Request request)
        {
            if (!Routes.TryGetValue(request.Path, out var func))
            {
                ResponseWriter.WriteStatus(System.Net.HttpStatusCode.NotFound, stream);
            }
            else
            {
                ResponseWriter.WriteStatus(System.Net.HttpStatusCode.OK, stream);
                WriteControllerResponse(func(), stream);
            }
        }

        /// <summary>
        /// Обработать (асинхронно)
        /// </summary>
        /// <param name="stream">Поток запроса</param>
        /// <param name="request">Информация о запросе</param>
        public async Task HandleAsync(Stream stream, Request request)
        {
            if (!Routes.TryGetValue(request.Path, out var func))
            {
                await ResponseWriter.WriteStatusAsync(System.Net.HttpStatusCode.NotFound, stream);
            }
            else
            {
                await ResponseWriter.WriteStatusAsync(System.Net.HttpStatusCode.OK, stream);
                WriteControllerResponse(func(), stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="stream"></param>
        private void WriteControllerResponse(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write(str);
            }
            else if (response is byte[] buffer)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                WriteControllerResponse(JsonConvert.SerializeObject(response), stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="stream"></param>
        private async Task WriteControllerResponseAsync(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync(str);
            }
            else if (response is byte[] buffer)
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            else if (response is Task task)
            {
                await task;
                await WriteControllerResponseAsync(task.GetType()
                                                       .GetProperty("Result")
                                                       .GetValue(task),
                                                   stream);
            }
            else
            {
                await WriteControllerResponseAsync(JsonConvert.SerializeObject(response), stream);
            }
        }
    }
}