using Data;
using Microsoft.Extensions.Configuration;

namespace Airport_Ticket_Management_System;

public abstract class SettingsLoader
{
    public static FilePathSettings LoadFileSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var filesPath = configuration.GetSection("FilesPath").Get<FilePathSettings>();
        if (filesPath is null) throw new NullReferenceException("Files path is null");
        return filesPath;
    }
}