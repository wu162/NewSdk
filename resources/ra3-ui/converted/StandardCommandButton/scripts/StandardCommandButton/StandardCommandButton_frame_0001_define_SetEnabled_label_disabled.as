function SetEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   if(_loc1_)
   {
      gotoAndStop("_up");
      play();
   }
   else
   {
      if(isMouseOver)
      {
         RollOver.gotoAndPlay("_hide");
         RollOverTop.gotoAndPlay("_hide");
      }
      gotoAndStop("_disabled");
      play();
   }
   isEnabled = _loc1_;
}
function SetVisibility(visArg)
{
   _visible = Number(visArg) != 0;
}
function SetNumberVisibility(visArg)
{
   numberText._visible = Number(visArg) != 0;
}
function SetSelected(selectArg)
{
   var _loc1_ = Number(selectArg) != 0;
   isSelected = _loc1_;
   trace("IS_SELECTED:  " + isSelected);
   if(isSelected)
   {
      selectedOverlay.Show();
   }
   else
   {
      selectedOverlay.Hide();
   }
}
function SetHighlighted(highlightArg)
{
   trace("StandardCommandButton.swf SetHighlighted( " + String(highlightArg) + " )");
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
function SetTimerVisibility(visArg)
{
   timerVisible = Number(visArg) != 0;
   timerOverlay0._visible = timerVisible;
   timerOverlay1._visible = timerVisible;
}
function SetTimerValue(valArg)
{
   var _loc2_ = Number(valArg);
   var _loc1_ = Math.round(_loc2_ * 100) + 1;
   timerOverlay0.gotoAndStop(_loc1_);
   timerOverlay1.gotoAndStop(_loc1_);
}
function SetImageTint(tintColorArg)
{
   var _loc1_ = Number(tintColorArg);
   imageTint = _loc1_;
   if(image.colorizer != undefined)
   {
      image.colorizer.setRGB(imageTint);
   }
}
function SetHealthBarVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   healthBar._visible = _loc1_;
}
function SetHealthBarValue(valueArg)
{
   var _loc1_ = Number(valueArg);
   healthBar.gotoAndStop(_loc1_ * 100 + 1);
}
function UseAlternateColors(arg)
{
   useAlternateColor = Number(arg) != 0;
   var _loc1_ = !useAlternateColor ? "_normal" : "_alternate";
   bg.image.gotoAndPlay(_loc1_);
   frame.image.gotoAndPlay(_loc1_);
}
function SetAmphibious(arg)
{
   amphibiousOverlay._visible = arg != 0;
}
function SetInfiltrated(arg)
{
   infiltratedOverlay._visible = arg != 0;
}
function onUnload()
{
   highlightClip.Close();
   delete SetEnabled;
   delete SetVisibility;
   delete SetNumberVisibility;
   delete SetSelected;
   delete SetTimerVisibility;
   delete SetTimerValue;
   delete SetImageTint;
   delete SetHealthBarVisibility;
   delete SetHealthBarValue;
   delete UseAlternateColors;
   delete SetAmphibious;
   delete SetInfiltrated;
   delete onUnload;
}
if(!extern.InGame)
{
   _root.GetCurrentFaction = function()
   {
      return "Gdi";
   };
   gotoAndStop("_up");
   play();
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   healthBar._visible = false;
   useAlternateColor = false;
   isEnabled = false;
   isSelected = false;
   isMouseOver = false;
   timerVisible = false;
   numberText._visible = false;
   imageTint = 16777215;
   healthBar.gotoAndStop(101);
   amphibiousOverlay._visible = false;
   infiltratedOverlay._visible = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      isEnabled = true;
      SetHealthBarVisibility(1);
      SetHealthBarValue(random(100) / 100);
   }
}
