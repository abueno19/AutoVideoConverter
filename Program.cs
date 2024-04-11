using System.Threading.Channels;
using AutoVideoConverter.Utils;

namespace AutoVideoConverter;

class Program
{
    
    static void Main(string[] args)
    {
        var fileSearch = FileSearch.GetInstance("./data");
        
        Converter converter = Converter.GetInstance();
        while(true){
            fileSearch.SearchMKVFiles(null).ForEach(files =>{
                converter.ConvertToMp4WithSubtitles(files);
            });
            Thread.Sleep(10000);
        }
    }
}
