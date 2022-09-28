// See https://aka.ms/new-console-template for more information


namespace ComputarAutoLens
{
    public class Program
    {
        private const string srcFolder = "C:\\ProgramData\\AutoFocus\\";
        private const string imageAcquisitionFolder = "ImageProc\\";
        private const string lensConfig = "LensConfig\\";
        private const string depthSensor = "DepthSensor\\";
        static int Main(string[] args)
        {
            InitializeFolderHierachy();
            return 1;
        }

        static void InitializeFolderHierachy()
        {
            // root folder
            if (!Directory.Exists(srcFolder))
            {
                Directory.CreateDirectory(srcFolder);
            }

            // sub folders
            if (!Directory.Exists(srcFolder + imageAcquisitionFolder))
            {
                Directory.CreateDirectory(srcFolder + imageAcquisitionFolder);
            }

            if (!Directory.Exists(srcFolder + lensConfig))
            {
                Directory.CreateDirectory(srcFolder + lensConfig);
            }

            if (!Directory.Exists(srcFolder + depthSensor))
            {
                Directory.CreateDirectory(srcFolder + depthSensor);
            }
        }
    }
}