using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityCiPipeline {
	public class CustomBuildPipeline : MonoBehaviour {
		[MenuItem("BuildPipeline/RunBuild")]
		public static void RunBuild(BuildTarget target) {
			var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
			var opts = new BuildPlayerOptions {
				target           = target,
				targetGroup      = targetGroup,
				locationPathName = "Build",
				scenes           = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray()
			};
			BuildPipeline.BuildPlayer(opts);
		}

		[MenuItem("BuildPipeline/RunBuild/WebGL")]
		public static void RunBuild_WebGL() => RunBuild(BuildTarget.WebGL);

		public static void RunBuildForVersion() {
			var targetVariable = Environment.GetEnvironmentVariable("BUILD_TARGET");
			if ( string.IsNullOrEmpty(targetVariable) ) {
				throw new InvalidOperationException("BUILD_TARGET isn't provided!");
			}
			var target = (BuildTarget)Enum.Parse(typeof(BuildTarget), targetVariable);
			var commitHash = GetVersion();
			Debug.Log($"RunBuildForCommit: version='{commitHash}'");
			PrepareForBuild(commitHash);
			RunBuild(target);
		}

		static string GetVersion() {
			return Environment.GetCommandLineArgs()
				.Where(a => a.StartsWith("-version="))
				.Select(a => a.Remove(0, "-version=".Length))
				.First();
		}

		static void PrepareForBuild(string version) {
			PlayerSettings.bundleVersion = version;
		}
	}
}
