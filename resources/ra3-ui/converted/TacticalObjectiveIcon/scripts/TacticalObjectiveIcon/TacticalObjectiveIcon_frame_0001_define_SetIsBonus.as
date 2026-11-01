function SetIsBonus(val)
{
   isBonus = val;
   icon.gotoAndPlay(!isBonus ? "_primary" : "_bonus");
}
function SetOrdinalText(val)
{
   marker.textClip.textVar = val;
}
function onUnload()
{
   delete SetIsBonus;
   delete SetOrdinalText;
   delete onUnload;
}
if(!initialized)
{
   isBonus = false;
   _parent.OnContentLoaded(this);
   initialized = true;
}
