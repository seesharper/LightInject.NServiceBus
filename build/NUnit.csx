#load "NuGet.csx"
#load "FileUtils.csx"
#load "OpenCover.csx"
#load "Context.csx"
#load "CodeCov.csx"
public static class NUnit
{
    private static readonly string pathToTestRunner;

    static NUnit()
    {
        NuGet.Install("NUnit.ConsoleRunner");    
        pathToTestRunner = FileUtils.FindFile(BuildContext.BuildPackagesFolder, "nunit3-console.exe");
    }

    public static void AnalyzeCodeCoverage(string pathToTestAssembly, string filter)
    {
        string pathToCoverageFile = Path.Combine(Path.GetDirectoryName(pathToTestAssembly),"coverage.xml");
        var testRunnerArgs = $"{pathToTestAssembly}";
        OpenCover.Execute(pathToTestRunner, testRunnerArgs, pathToCoverageFile, filter);
        CodeCov.Upload(pathToCoverageFile);
    }
}