#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.0

var target = Argument("target", string.Empty);
var buildDir = Directory("./Build");

Func<string, string, string> Run = (fileName, cmd) => {
	var settings = new ProcessSettings {
		RedirectStandardOutput = true,
		RedirectStandardError  = true,
		Arguments              = new ProcessArgumentBuilder().Append(cmd)
	};
	Information($"Run program '{fileName}' with command: '{cmd}'");
	using ( var proc = StartAndReturnProcess(fileName, settings) ) {
		proc.WaitForExit();
		var stdout = string.Join("\n", proc.GetStandardOutput());
		Information($"Stdout:\n{stdout}");
		Information($"Stderr:\n{string.Join("\n", proc.GetStandardError())}");
		var exitCode = proc.GetExitCode();
		if ( exitCode != 0 ) {
			throw new Exception($"Exit code: {exitCode}");
		}
		return stdout;
	}
};

Func<string> GetRequiredUnityVersion = () => {
	var versionPath = "ProjectSettings/ProjectVersion.txt";
	var prefix = "m_EditorVersion: ";
	var lines = FileReadLines(versionPath);
	return lines
		.Where(l => l.StartsWith(l))
		.Select(l => l.Substring(prefix.Length))
		.First();
};

Task("Clean")
	.Does(() =>
{
	CleanDirectory(buildDir);
});

Task("Install-Unity")
	.Does(() =>
{
	var unityVersion = GetRequiredUnityVersion();
	Information($"Required Unity version is '{unityVersion}'");
	Run("u3d", $"install {unityVersion}");
});


Task("Build")
	.IsDependentOn("Clean")
	.Does(() =>
{
	var unityVersion = GetRequiredUnityVersion();
	var unityPath = $"/Applications/Unity_{unityVersion}";
	Run(unityPath, "-quit -batchmode -logFile - -executeMethod UnityCiPipeline.CustomBuildPipeline.RunBuildForVersion -projectPath . -version=1.2.3.travis");
});

RunTarget(target);
