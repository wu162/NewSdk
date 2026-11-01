function Start()
{
   stop();
   cyclesLeft = cycleCount;
   currentArrowNum = 0;
   var _loc2_ = this[String(currentArrowNum)];
   _loc2_.gotoAndPlay("_flash");
   _visible = true;
}
function Stop()
{
   var _loc2_;
   if(cyclesLeft > 0)
   {
      _loc2_ = this[String(currentArrowNum)];
      _loc2_.gotoAndPlay("_off");
      cyclesLeft = 0;
      _visible = false;
   }
}
function NextFlash()
{
   if(cyclesLeft <= 0)
   {
      return undefined;
   }
   currentArrowNum++;
   if(currentArrowNum >= arrowCount)
   {
      cyclesLeft--;
      sunburst.gotoAndPlay("_flash");
      if(cyclesLeft <= 0)
      {
         _visible = false;
         return undefined;
      }
      currentArrowNum = 0;
   }
   var _loc2_ = this[String(currentArrowNum)];
   _loc2_.gotoAndPlay("_flash");
}
function onUnload()
{
   delete Start;
   delete Stop;
   delete NextFlash;
   delete onUnload;
}
if(!initialized)
{
   _visible = false;
   arrowCount = 5;
   cycleCount = 5;
   var i = 0;
   while(i < arrowCount)
   {
      var arrowClip = this[String(i)];
      arrowClip.client = this;
      i++;
   }
   initialized = true;
   if(!extern.InGame)
   {
      onEnterFrame = function()
      {
         Start();
         delete onEnterFrame;
      };
   }
}
stop();
