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
