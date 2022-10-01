// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SaveDataToJson
{
    public class Program
    {
        //private const string srcFolder = "C:\\ProgramData\\AutoFocus\\DepthSensor\\";
        //private const string depthSensorData = "ds_db.json";
        //private const string lensData = "lens_db.json";

        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid input params.");
            }

            string savePath = args[0];
            string saveValue = args[1];

            //string savePath = "C:\\ProgramData\\AutoFocus\\DepthSensor\\ds_db.json";
            //string saveData = "10;20;30;40;10;20;30;40;20;50";

            SaveDataToFile(savePath, saveValue);

            return 1;
        }

        static void SaveDataToFile(string filename, string data)
        {
            //if (!File.Exists(filename))
            //{
            //    File.Create(filename);
            //}
            

            var dataSplit = data.Split(";");
            var dataObjs = new List<DataObj>();
            for (int i = 0; i < dataSplit.Length; i++)
            {
                DataObj newObj = new DataObj(dataSplit[i], i.ToString());
                dataObjs.Add(newObj);
            }

            var dataWrapper = new DataWrapper(dataObjs);

            // Convert dataObjs to json obj
            string jsonString = JsonSerializer.Serialize<DataWrapper>(dataWrapper);
            File.WriteAllText(filename, jsonString);
        }
    }
}
