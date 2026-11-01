function Resize(newWidth, newHeight)
{
   sizer._width = newWidth;
   sizer._height = newHeight;
   var _loc2_;
   _loc2_ = 0;
   var _loc1_;
   while(_loc2_ < pings.length)
   {
      _loc1_ = pings[_loc2_];
      if(_loc1_ != undefined && _loc1_.relX != undefined && _loc1_.relY != undefined)
      {
         _loc1_._x = _loc1_.relX * newWidth;
         _loc1_._y = _loc1_.relY * newHeight;
      }
      _loc2_ = _loc2_ + 1;
   }
}
function CreatePing(type, pingColor)
{
   var _loc2_ = nextPingId++;
   var _loc1_ = attachMovie(type,String(_loc2_),_loc2_);
   _loc1_.relX = 0;
   _loc1_.relY = 0;
   _loc1_.pingColor = pingColor;
   pings.push(_loc1_);
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function DetachPing(ping)
{
   if(ping != undefined)
   {
      ping.done = true;
   }
}
function MovePing(ping, relX, relY)
{
   if(ping != undefined)
   {
      ping.relX = relX;
      ping.relY = relY;
      ping._x = relX * sizer._width;
      ping._y = relY * sizer._height;
   }
}
function DestroyPing(ping)
{
   var _loc1_ = 0;
   while(_loc1_ < pings.length)
   {
      if(pings[_loc1_] == ping)
      {
         pings.splice(_loc1_,1);
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
   ping.removeMovieClip();
   if(pings.length <= 0)
   {
      nextPingId = 0;
   }
}
function onUnload()
{
   delete Resize;
   delete CreatePing;
   delete DetachPing;
   delete MovePing;
   delete DestroyPing;
   delete onUnload;
}
inGame = extern.InGame;
if(!initialized)
{
   nextPingId = 0;
   pings = new Array();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      MovePing(CreatePing("PingPlayer",4278190336),0.5,0.5);
      MovePing(CreatePing("PingBeacon"),0.25,0.25);
      MovePing(CreatePing("PingUpgrade"),0.75,0.25);
   }
}
stop();
