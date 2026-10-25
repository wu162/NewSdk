using System.Collections.Generic;

namespace EALABuild;

public interface EALABuildScript
{
	void initialize();

	void onStepSuccess(int stepId);

	void onStepFailure(int stepId);

	void buildMod(string modName, Dictionary<string, object> GUIResults);

	bool shouldExecuteStep(int stepId);

	List<GUIElementData> GetModBuildGUIData();
}
