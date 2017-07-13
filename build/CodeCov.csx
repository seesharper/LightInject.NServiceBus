#load "NuGet.csx"
#load "FileUtils.csx"
#load "Command.csx"
#load "Context.csx"
public static class CodeCov
{
    private static readonly string pathToCodeCov;
    static CodeCov()
    {
        NuGet.Install("CodeCov");
        pathToCodeCov = FileUtils.FindFile(BuildContext.BuildPackagesFolder, "codecov.exe");
    }

    public static void Upload(string pathToCoverageFile)
    {
        Command.Execute(pathToCodeCov, $"-f \"{pathToCoverageFile}\" -t 3ebbb2c0-3e34-4299-a857-c0432df3c944", ".");
    }
}