function SetLeftButtonEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   leftButton.gotoAndPlay(!_loc1_ ? "_disabled" : "_up");
}
function SetRightButtonEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   rightButton.gotoAndPlay(!_loc1_ ? "_disabled" : "_up");
}
function onUnload()
{
   delete SetLeftButtonEnabled;
   delete SetRightButtonEnabled;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
