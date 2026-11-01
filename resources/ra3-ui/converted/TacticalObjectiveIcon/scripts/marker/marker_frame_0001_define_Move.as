function Move(x, y)
{
   var _loc3_ = _parent;
   while(_loc3_ != _root)
   {
      x -= _loc3_._x;
      y -= _loc3_._y;
      _loc3_ = _loc3_._parent;
   }
   _X = x;
   _Y = y;
}
function SetVisibility(visibleArg)
{
   _visible = visibleArg != 0;
}
function IndicateNewness()
{
   newness.gotoAndPlay("_play");
}
function GetPopUpStage()
{
   return Pointer.encode(popUpStage);
}
function onUnload()
{
   delete Move;
   delete SetVisibility;
   delete IndicateNewness;
   delete GetPopUpStage;
   delete onUnload;
}
if(!initialized)
{
   primary.gotoAndPlay(category);
   bonus.gotoAndPlay(category);
   if(isBonus)
   {
      primary._visible = false;
   }
   else
   {
      bonus._visible = false;
   }
   newness.gotoAndStop(1);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      IndicateNewness();
   }
}
stop();
