#load "NuGet.csx"
#load "FileUtils.csx"
#load "Context.csx"
public static class OpenCover
{    
    private static string pathToOpenCover;
    private static string pathToReportGenerator;
    static OpenCover()
    {
        NuGet.Install("OpenCover");
        NuGet.Install("ReportGenerator");
        pathToOpenCover = FileUtils.FindFile(BuildContext.BuildPackagesFolder,"OpenCover.Console.exe");
        pathToReportGenerator = FileUtils.FindFile(BuildContext.BuildPackagesFolder,"ReportGenerator.exe");
    }

    public static void Execute(string pathToTestRunner,string testRunnerArgs, string pathToCoverageFile, string filter)
    {        
        var args = $"-target:\"{pathToTestRunner}\" -targetargs:\"{testRunnerArgs}\" -output:\"{pathToCoverageFile}\" -filter:\"{filter}\" -register:user";
        Command.Execute(pathToOpenCover, args, ".");
        CreateSummaryFile(pathToCoverageFile);
    }

    private static void CreateSummaryFile(string pathToCoverageFile)
    {
        //var filters = includedAssemblies.Select (a => "+" + a).Aggregate ((current, next) => current + ";" + next).ToLower();
        var targetDirectory = Path.GetDirectoryName(pathToCoverageFile);
        var args = $"-reports:\"{pathToCoverageFile}\" -targetdir:\"{targetDirectory}\" -reporttypes:xmlsummary";
        Command.Execute(pathToReportGenerator, args);
    }
}