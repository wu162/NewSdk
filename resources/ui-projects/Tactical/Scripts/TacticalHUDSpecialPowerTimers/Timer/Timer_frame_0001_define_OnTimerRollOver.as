function OnTimerRollOver(timer)
{
   if(timer == childStage.child)
   {
      overChild = true;
   }
}
function OnTimerRollOut(timer)
{
   if(timer == childStage.child)
   {
      overChild = false;
   }
}
function SetID(id)
{
   timeText = "$" + qualifyName(id,"Time") + "&outline";
   timerText.gotoAndPlay(1);
   image._imageMap = qualifyName(id,"Image");
}
function onUnload()
{
   delete OnTimerRollOver;
   delete OnTimerRollOut;
   delete SetID;
   delete onEnterFrame;
   delete onUnload;
}
if(!initialized)
{
   overChild = false;
   innerFrameColorizer = new Color(innerFrame);
   initialized = true;
   if(client != undefined)
   {
      client.OnTimerInitialized(this);
   }
}
stop();
onEnterFrame = function()
{
   if(overChild)
   {
      delete onEnterFrame;
      gotoAndStop("_down");
      play();
   }
};
