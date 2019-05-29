#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

var target = Argument<string>("target");
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
