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

Func<string[], string, string> GetStrStartsWith = (lines, prefix) => {
	return lines
		.Select(l => l.Trim())
		.Where(l => l.StartsWith(prefix))
		.Select(l => l.Substring(prefix.Length))
		.First();
};

Func<string> GetRequiredUnityVersion = () => {
	return GetStrStartsWith(
		FileReadLines("ProjectSettings/ProjectVersion.txt"),
		"m_EditorVersion: ");
};

Func<string> GetLatestCommit = () => {
	return Run("git", "rev-parse --short HEAD");
};

Func<string> GetProjectVersion = () => {
	return GetStrStartsWith(
		FileReadLines("ProjectSettings/ProjectSettings.asset"),
		"bundleVersion: ");
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
	.Does(() =>
{
	var unityVersion = GetRequiredUnityVersion();
	var unityPath = $"/Applications/Unity_{unityVersion}/Unity.app/Contents/MacOS/Unity";
	var latestCommit = GetLatestCommit();
	var version = GetProjectVersion() + "." + latestCommit;
	var userName = Environment.GetEnvironmentVariable("UNITY_USERNAME");
	var password = Environment.GetEnvironmentVariable("UNITY_PASSWORD");
	var cmd = "-quit -batchmode -nographics -logFile - -executeMethod UnityCiPipeline.CustomBuildPipeline.RunBuildForVersion -projectPath . ";
	cmd += $"-version={version} -username {userName} -password {password} -force-free";
	Run(unityPath, cmd);
});

Task("Upload")
	.Does(() =>
{
	var version = GetProjectVersion();
	var target = Environment.GetEnvironmentVariable("ITCH_TARGET");
	Run("butler", $"push --userversion={version} --verbose Build {target}");
});

RunTarget(target);
