function OnPress()
{
   gotoAndStop("down");
   play();
}
function OnRelease()
{
   gotoAndStop("over");
   play();
   fscommand(_global.qualifyName(this,"OnClicked"),"");
}
function OnRollOver()
{
   gotoAndStop("over");
   play();
   fscommand(_global.qualifyName(this,"OnRollOver"),"");
}
function OnRollOut()
{
   gotoAndStop("up");
   play();
   fscommand(_global.qualifyName(this,"OnRollOut"),"");
}
function SetSelected(selectedArg)
{
   imageMC.gotoAndPlay(selectedArg == 0 ? "normal" : "selected");
}
function GetHelpSite()
{
   return Pointer.encode(helpSiteMC);
}
function onUnload()
{
   delete OnPress;
   delete OnRelease;
   delete OnRollOver;
   delete OnRollOut;
   delete SetSelected;
   delete GetHelpSite;
   delete onUnload;
}
if(!initialized)
{
   hitAreaButton.onPress = _global.bind0(this,OnPress);
   hitAreaButton.onRelease = _global.bind0(this,OnRelease);
   hitAreaButton.onRollOver = _global.bind0(this,OnRollOver);
   hitAreaButton.onRollOut = _global.bind0(this,OnRollOut);
   initialized = true;
   fscommand(_global.qualifyName(this,"OnInitialized"),"");
}
