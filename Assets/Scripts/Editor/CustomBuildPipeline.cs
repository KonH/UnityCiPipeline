using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityCiPipeline {
	public class CustomBuildPipeline : MonoBehaviour {
		[MenuItem("BuildPipeline/RunBuild")]
		public static void RunBuild() {
			var opts = new BuildPlayerOptions {
				target           = BuildTarget.WebGL,
				targetGroup      = BuildTargetGroup.WebGL,
				locationPathName = "Build"
			};
			BuildPipeline.BuildPlayer(opts);
		}

		public static void RunBuildForVersion() {
			var commitHash = GetVersion();
			Debug.Log($"RunBuildForCommit: version='{commitHash}'");
			PrepareForBuild(commitHash);
			RunBuild();
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
