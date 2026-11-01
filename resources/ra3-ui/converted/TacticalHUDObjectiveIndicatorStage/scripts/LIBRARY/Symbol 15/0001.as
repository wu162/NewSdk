function SetObjectivePos(objectiveX, objectiveY)
{
   var _loc3_ = this;
   while(_loc3_ != _root)
   {
      objectiveX -= _loc3_._x;
      objectiveY -= _loc3_._y;
      _loc3_ = _loc3_._parent;
   }
   var _loc6_ = Math.atan2(objectiveY,objectiveX);
   _loc6_ *= 57.295780181884766;
   _rotation = _loc6_;
   ordinal._rotation = - _loc6_;
   helpSite.vertAlignment = objectiveY < 0 ? "top" : "bottom";
   helpSite.horzAlignment = objectiveX < 0 ? "left" : "right";
}
function SetVisibility(visibleArg)
{
   _visible = visibleArg != 0;
}
function IndicateNewness()
{
   newness.gotoAndPlay("_play");
}
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function SetBonus(bonus)
{
   if(bonus)
   {
      marker.SetBonus();
   }
}
function onUnload()
{
   delete SetObjectivePos;
   delete SetVisibility;
   delete IndicateNewness;
   delete GetHelpSite;
   delete SetBonus;
   delete onUnload;
}
if(inGame == undefined)
{
   inGame = extern.InGame;
}
if(!initialized)
{
   _alpha = 50;
   gotoAndPlay(category);
   if(isBonus)
   {
      marker.SetBonus();
   }
   newness.gotoAndStop(1);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!inGame)
   {
      SetObjectivePos(Math.random() * 1024,Math.random() * 768);
      IndicateNewness();
   }
}
