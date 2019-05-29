#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

var target = Argument("target", string.Empty);
var buildDir = Directory("./Build");

Task("Clean")
    .Does(() =>
{
	CleanDirectory(buildDir);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{

});

RunTarget(target);
