#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.0

var target = Argument("target", string.Empty);
var buildDir = Directory("./Build");

Func<string, string, bool, string> Run = (fileName, cmd, ignoreExitCode) => {
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
		Information($"Exit code is {exitCode}");
		if ( (exitCode != 0) && !ignoreExitCode ) {
			throw new Exception($"Run '{fileName}' failed.");
		}
		return stdout;
	}
};

Func<string, bool, string> RunUnity = (cmd, ignoreExitCode) => {
	var unityVersion = GetRequiredUnityVersion();
	var unityPath = $"/Applications/Unity_{unityVersion}/Unity.app/Contents/MacOS/Unity";
	var userName = Environment.GetEnvironmentVariable("UNITY_USERNAME");
	var password = Environment.GetEnvironmentVariable("UNITY_PASSWORD");
	var fullCmd = cmd + $" -quit -batchmode -nographics -logFile - -username {userName} -password {password}";
	return Run(unityPath, fullCmd, ignoreExitCode);
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
	return Run("git", "rev-parse --short HEAD", false);
};

Func<string> GetProjectVersion = () => {
	return GetStrStartsWith(
		FileReadLines("ProjectSettings/ProjectSettings.asset"),
		"bundleVersion: ");
};

Func<string> GetManualLicenseFileName = () => {
	return "Unity_lic.ulf";
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
	Run("u3d", $"install {unityVersion}", false);
});

Task("Retrieve-Manual-Activation-File")
	.Does(() =>
{
	var unityVersion = GetRequiredUnityVersion();
	var activationFileName = $"Unity_v{unityVersion}.alf";
	Information($"Expected activation file name: {activationFileName}");
	RunUnity("-createManualActivationFile", true);
	Information($"Activation file content: {FileReadText(activationFileName)}");
});

Task("Perform-Manual-Activation")
	.Does(() =>
{
	var unityVersion = GetRequiredUnityVersion();
	var activationFileName = $"Unity_v{unityVersion}.alf";
	Information($"Save {activationFileName} as common unity3d.alf");
	CopyFile(activationFileName, "unity3d.alf");

	Run("node", "manual_activation.js", false);

	var licenseFileName = GetManualLicenseFileName();
	if ( FileExists(licenseFileName) ) {
		throw new Exception("Can't find license file!");
	}
});

Task("Return-License")
	.Does(() =>
{
	RunUnity("-returnLicense", false);
});

Task("Build")
	.Does(() =>
{
	var latestCommit = GetLatestCommit();
	var version = GetProjectVersion() + "." + latestCommit;
	RunUnity($"-executeMethod UnityCiPipeline.CustomBuildPipeline.RunBuildForVersion -projectPath . -version={version}", false);
});

Task("Upload")
	.Does(() =>
{
	var version = GetProjectVersion();
	var target = Environment.GetEnvironmentVariable("ITCH_TARGET");
	Run("butler", $"push --userversion={version} --verbose Build {target}", false);
});

RunTarget(target);
