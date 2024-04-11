using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Channels;
using GroupDocs.Metadata;

public class FileSearch
{
    private static FileSearch instance;
    private static readonly object lockObject = new object();

    private FileSearch()
    {
        // Constructor privado para evitar la creación de instancias directamente
    }

    private static FileSearch Instance{get;set;}
    
    private string directory{get;set;}
    
    private FileSearch(String directory){
        this.directory = directory;
        
    }
    public static FileSearch GetInstance(String directory){
        if(Instance == null){
            Instance = new FileSearch(directory);
        }
        return Instance;
    }

    public List<string> SearchMKVFiles(string? directoryPath )
    {
        List<string> mkvFiles = new List<string>();

        try
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath ?? this.directory ?? throw new ArgumentNullException("directoryPath"));
            FileInfo[] files = directory.GetFiles("*.mkv", SearchOption.AllDirectories);
            // Vamos a editar el metadata de los archivos MKV para marcarlos como "en proceso"
            mkvFiles.AddRange(files.Select(f => f.FullName));

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al buscar archivos MKV: " + ex.Message);
        }

        return mkvFiles;
    }
}