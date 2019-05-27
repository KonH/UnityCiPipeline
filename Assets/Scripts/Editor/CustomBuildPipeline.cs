using UnityEditor;
using UnityEngine;

public class CustomBuildPipeline : MonoBehaviour {
	[MenuItem("BuildPipeline/RunBuild")]
	public static void RunBuild() {
		var opts = new BuildPlayerOptions {
			target           = BuildTarget.WebGL,
			targetGroup      = BuildTargetGroup.WebGL,
			locationPathName = "Build"
		};
		BuildPipeline.BuildPlayer(opts);
		
		// later:
		// butler push /Volumes/SamsungSSD/Projects/UnityCiPipeline/Build konh/test-ci:html
	}
}
