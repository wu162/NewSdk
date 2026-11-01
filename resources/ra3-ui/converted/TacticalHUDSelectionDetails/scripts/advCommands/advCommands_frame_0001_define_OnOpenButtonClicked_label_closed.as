function OnOpenButtonClicked()
{
   fscommand(_global.qualifyName(_parent,"OnOpenAdvancedCommandsClicked"),"");
}
function OnRollOverOpenButton()
{
   if(!mouseOverOpenButton)
   {
      mouseOverOpenButton = true;
      fscommand(_global.qualifyName(_parent,"OnRollOverOpenAdvancedCommandsButton"),"");
   }
}
function OnRollOutOpenButton()
{
   if(mouseOverOpenButton)
   {
      mouseOverOpenButton = false;
      fscommand(_global.qualifyName(_parent,"OnRollOutOpenAdvancedCommandsButton"),"");
   }
}
function onUnload()
{
   delete OnOpenButtonClicked;
   delete OnRollOverOpenButton;
   delete OnRollOutOpenButton;
   delete onUnload;
}
if(!initialized)
{
   mouseOverOpenButton = false;
   initialized = true;
}
stop();
