function SetEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   if(_loc1_ != isEnabled)
   {
      isEnabled = _loc1_;
      box.gotoAndPlay(!_loc1_ ? "_disabled" : (!isOver ? "_up" : "_over"));
   }
}
function SetChecked(checkedArg)
{
   var _loc1_ = Number(checkedArg) != 0;
   gotoAndPlay(!_loc1_ ? "_unchecked" : "_checked");
}
function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   _visible = _loc1_;
}
function OnClicked(clip)
{
   fscommand(qualifyName(this,"OnClicked"),"");
}
function OnRollOver(clip)
{
   fscommand(qualifyName(this,"OnRollOver"),"");
}
function OnRollOut(clip)
{
   fscommand(qualifyName(this,"OnRollOut"),"");
}
function onUnload()
{
   delete SetEnabled;
   delete SetChecked;
   delete SetVisibility;
   delete OnClicked;
   delete OnRollOver;
   delete OnRollOut;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   isEnabled = true;
   isOver = false;
   if(client == undefined)
   {
      client = this;
   }
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
