using Data;
using Microsoft.Extensions.Configuration;

namespace Airport_Ticket_Management_System;

public class SettingsLoader
{
    public static FilePathSettings LoadFileSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("C:/Users/Ruba/OneDrive/Desktop/BE/Airport_Ticket_Management_System/Airport_Ticket_Management_System/appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var filesPath = configuration.GetSection("FilesPath").Get<FilePathSettings>();
        if (filesPath is null) throw new NullReferenceException("Files path is null");
        return filesPath;
    }
}