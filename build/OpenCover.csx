#load "NuGet.csx"
#load "FileUtils.csx"
#load "Context.csx"
public static class OpenCover
{    
    private static string pathToOpenCover;
    static OpenCover()
    {
        NuGet.Install("OpenCover");
        pathToOpenCover = FileUtils.FindFile(BuildContext.BuildPackagesFolder,"OpenCover.Console.exe");
    }

    public static void Execute(string pathToTestRunner,string testRunnerArgs, string pathToCoverageFile, string filter)
    {
        Command.Execute(pathToTestRunner, $"\"{testRunnerArgs}\"");        
        var args = $"-target:\"{pathToTestRunner}\" -targetargs:\"{testRunnerArgs}\" -output:\"{pathToCoverageFile}\" -filter:\"{filter}\" -register:user";
        Command.Execute(pathToOpenCover, args, ".");
    }
}