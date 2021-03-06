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
	var unityPath = $"/opt/unity-editor-{unityVersion}/Editor/Unity";
	var userName = Environment.GetEnvironmentVariable("UNITY_USERNAME");
	if ( string.IsNullOrEmpty(userName) ) {
		throw new Exception("No UNITY_USERNAME provided!");
	}
	var password = Environment.GetEnvironmentVariable("UNITY_PASSWORD");
	if ( string.IsNullOrEmpty(password) ) {
		throw new Exception("No UNITY_PASSWORD provided!");
	}
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
		System.IO.File.ReadAllLines("ProjectSettings/ProjectVersion.txt"),
		"m_EditorVersion: ");
};

Func<string> GetLatestCommit = () => {
	return Run("git", "rev-parse --short HEAD", false);
};

Func<string> GetProjectVersion = () => {
	return GetStrStartsWith(
		System.IO.File.ReadAllLines("ProjectSettings/ProjectSettings.asset"),
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
	Information("Show help:");
	Run("u3d", "--help", false);
	Information("Available Unity versions:");
	Run("u3d", "available -f --no-central --trace --verbose", false);
	var unityVersion = GetRequiredUnityVersion();
	Information($"Required Unity version is '{unityVersion}'");
	var installArgs = Environment.GetEnvironmentVariable("INSTALL_ARGS");
	if ( string.IsNullOrEmpty(installArgs) ) {
		installArgs = "Unity,WebGL";
	}
	Information("Install wanted Unity version:");
	Run("u3d", $"install {unityVersion} -p {installArgs} --trace --verbose", false);
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
	var buildTarget = Environment.GetEnvironmentVariable("BUILD_TARGET");
	if ( string.IsNullOrEmpty(buildTarget) ) {
		buildTarget = "WebGL";
	}
	RunUnity($"-executeMethod UnityCiPipeline.CustomBuildPipeline.RunBuildForVersion -projectPath . -version={version} -buildTarget={buildTarget}", false);
});

Task("Upload")
	.Does(() =>
{
	var version = GetProjectVersion();
	var target = Environment.GetEnvironmentVariable("ITCH_TARGET");
	if ( string.IsNullOrEmpty(target) ) {
		throw new Exception("No ITCH_TARGET provided!");
	}
	Run("butler", $"push --userversion={version} --verbose Build {target}", false);
});

Task("Retrieve-Manual-Activation-File")
	.Does(() =>
{
	var unityVersion = GetRequiredUnityVersion();
	var activationFileName = $"Unity_v{unityVersion}.alf";
	Information($"Expected activation file name: {activationFileName}");
	RunUnity("-createManualActivationFile", true);
	Information($"Activation file content:\n{System.IO.File.ReadAllText(activationFileName)}");
});

Task("Encode-License-File")
	.Does(() =>
{
	var fileName = Argument("fileName", "Unity_lic.ulf");
	if ( !FileExists(fileName) ) {
		throw new Exception($"Can't find '{fileName}'");
	}
	var content = System.IO.File.ReadAllText(fileName);
	var bytes = Encoding.UTF8.GetBytes(content);
	var base64 = Convert.ToBase64String(bytes);
	Information(base64);
});

Task("Decode-License-File")
	.Does(() =>
{
	var base64 = Environment.GetEnvironmentVariable("UNITY_ULF");
	if ( string.IsNullOrEmpty(base64) ) {
		throw new Exception("No UNITY_ULF provided!");
	}
	var bytes = Convert.FromBase64String(base64);
	var content = Encoding.UTF8.GetString(bytes);
	var fileName = "Unity_lic.ulf";
	System.IO.File.WriteAllText(fileName, content);
	Information("Is license file exits? " + FileExists(fileName));
});

Task("Use-Manual-Activation-File")
	.Does(() =>
{
	RunUnity("-manualLicenseFile Unity_lic.ulf", true);
});

RunTarget(target);
