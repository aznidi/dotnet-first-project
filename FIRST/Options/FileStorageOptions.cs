using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIRST.Options;


public class FileStorageOptions
{
    public string RootPath { get; set; } = "App_Data";
    public string UploadsFolder { get; set; } = "uploads";
    public long MaxSizeBytes { get; set; } = 5 * 1024 * 1024; // 5MB
}