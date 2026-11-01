function SetEnabled(enabledArg)
{
   var _loc2_ = enabledArg != 0;
   var _loc3_;
   if(_loc2_ != isEnabled)
   {
      isEnabled = _loc2_;
      _loc3_ = new Color(this);
      _loc3_.setRGB(!isEnabled ? 8355711 : 16777215);
   }
}
function SetHighlighted(highlightArg)
{
   var _loc3_ = highlightArg != 0;
   var _loc2_;
   if(_loc3_ != isHighlighted)
   {
      isHighlighted = _loc3_;
      _loc2_ = _parent.playerTechStoreButtonHighlight;
      if(isHighlighted)
      {
         _loc2_.done = false;
         _loc2_.gotoAndPlay("on");
      }
      else
      {
         _loc2_.done = true;
      }
   }
}
function SetFlashing(flashingArg)
{
   var _loc2_ = flashingArg != 0;
   var _loc3_;
   if(_loc2_ != isFlashing)
   {
      isFlashing = _loc2_;
      _loc3_ = !_loc2_ ? "_off" : "_on";
      flashyThingy.gotoAndPlay(_loc3_);
      if(_loc2_)
      {
         _parent.ugly.Start();
      }
      else
      {
         _parent.ugly.Stop();
      }
   }
}
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function SetTechPoints(points)
{
   techPoints.gotoAndPlay("_" + String(points));
}
function onUnload()
{
   delete SetEnabled;
   delete SetHighlighted;
   delete SetFlashing;
   delete GetHelpSite;
   delete SetTechPoints;
   delete onUnload;
}
if(!initialized)
{
   isEnabled = true;
   isHighlighted = false;
   isFlashing = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      SetFlashing(1);
   }
}
