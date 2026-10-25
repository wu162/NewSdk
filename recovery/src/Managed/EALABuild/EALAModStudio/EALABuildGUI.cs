using System.Drawing;
using EALABuild;

namespace EALAModStudio;

public interface EALABuildGUI
{
	void runBuildStep(int stepId, StepType typeOfStep, object args);

	void displayLine(string text);

	void resetBuildLog();

	void onBuildComplete();

	void setColors(Color critical, Color error, Color warning);

	EALABuildScript getScriptInterface();
}
