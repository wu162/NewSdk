function SetEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   if(_loc1_)
   {
      bttn.gotoAndPlay("_up");
   }
   else
   {
      if(isMouseOver)
      {
         bttn.RollOver.gotoAndPlay("_hide");
         bttn.RollOverTop.gotoAndPlay("_hide");
      }
      bttn.gotoAndPlay("_disabled");
   }
   isEnabled = _loc1_;
}
function SetVisibility(visArg)
{
   bttn._visible = Number(visArg) != 0;
}
function SetTimerVisibility(vis)
{
   timerVisible = Number(vis) != 0;
   bttn.timerOverlay0._visible = timerVisible;
   bttn.timerOverlay1._visible = timerVisible;
}
function SetTimerValue(valArg)
{
   var _loc2_ = Number(valArg);
   var _loc1_ = Math.round(_loc2_ * 100) + 1;
   bttn.timerOverlay0.gotoAndStop(_loc1_);
   bttn.timerOverlay1.gotoAndStop(_loc1_);
}
function SetImageTint(tintColorArg)
{
   var _loc1_ = Number(tintColorArg);
   imageTint = _loc1_;
   if(bttn.image.colorizer != undefined)
   {
      bttn.image.colorizer.setRGB(imageTint);
   }
}
function SetQuantityVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   bttn.quantityText._visible = _loc1_;
}
function SetHighlighted(highlightArg)
{
   trace("TacticalCommandButton.swf SetHighlighted( " + String(highlightArg) + " )");
   var _loc3_ = highlightArg != 0;
   if(_loc3_)
   {
      if(highlightClip == undefined)
      {
         highlightClip = _global.CreateHUDGadgetFlash(this);
      }
   }
   else if(highlightClip != undefined)
   {
      highlightClip.Close();
      highlightClip = undefined;
   }
}
function onUnload()
{
   highlightClip.Close();
   delete SetEnabled;
   delete SetVisibility;
   delete SetTimerVisibility;
   delete SetTimerValue;
   delete SetImageTint;
   delete SetQuantityVisibility;
   delete onUnload;
}
if(!extern.InGame)
{
   _root.GetCurrentFaction = function()
   {
      return "Alien";
   };
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   isEnabled = false;
   isMouseOver = false;
   timerVisible = false;
   imageTint = 16777215;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      isEnabled = true;
   }
}
