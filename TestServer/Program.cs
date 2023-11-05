using ServerEngine;

//new ServerHost(new StaticFileHandler(Path.Combine(Environment.CurrentDirectory, "www"))).Start();
await (new ServerHost(new ControllerHandler(typeof(Program).Assembly)).StartAsync());
