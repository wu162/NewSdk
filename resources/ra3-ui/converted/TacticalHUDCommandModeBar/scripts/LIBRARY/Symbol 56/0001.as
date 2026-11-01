function SetState(newStateLabel)
{
   if(newStateLabel != stateLabel)
   {
      imageStage.clip.gotoAndPlay(newStateLabel);
      backgroundStage.clip.gotoAndPlay(newStateLabel);
      buttonStage.clip.gotoAndPlay(newStateLabel);
      stateLabel = newStateLabel;
   }
}
function OnPress()
{
   if(isEnabled)
   {
      SetState("_down");
   }
}
function OnRelease()
{
   if(isEnabled)
   {
      SetState("_over");
   }
   fscommand(qualifyName(this,"OnClicked"),"");
}
function OnRollOver()
{
   if(!isOver)
   {
      if(isEnabled)
      {
         SetState("_over");
      }
      isOver = true;
      fscommand(qualifyName(this,"OnRollOver"),"");
   }
}
function OnRollOut()
{
   if(isOver)
   {
      if(isEnabled)
      {
         SetState("_up");
      }
      isOver = false;
      fscommand(qualifyName(this,"OnRollOut"),"");
   }
}
function AttachContent()
{
   var _loc1_ = imageStage.clip;
   if(_loc1_ != undefined)
   {
      _loc1_.removeMovieClip();
   }
   var _loc2_;
   if(imageClipName != undefined && imageClipName.length > 0)
   {
      _loc2_ = imageClipName + (!isSelected ? "Normal" : "Highlighted");
      _loc1_ = imageStage.attachMovie(_loc2_,"clip",0);
      _loc1_.gotoAndPlay(stateLabel);
   }
   _loc1_ = backgroundStage.clip;
   if(_loc1_ != undefined)
   {
      _loc1_.removeMovieClip();
   }
   if(backgroundClipName != undefined && backgroundClipName.length > 0)
   {
      _loc1_ = backgroundStage.attachMovie(backgroundClipName + (!isSelected ? "Normal" : "Highlighted"),"clip",0);
      _loc1_.gotoAndPlay(stateLabel);
   }
}
function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   _visible = _loc1_;
   flashyThingy._visible = _loc1_;
}
function SetEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   if(_loc1_ != isEnabled)
   {
      isEnabled = _loc1_;
      SetState(!isEnabled ? "_disabled" : "_up");
   }
}
function SetSelected(selectArg)
{
   var _loc1_ = Number(selectArg) != 0;
   if(_loc1_ != isSelected)
   {
      isSelected = _loc1_;
      AttachContent();
   }
}
function SetFlashing(flashArg)
{
   if(flashyThingy == undefined)
   {
      return undefined;
   }
   var _loc1_ = Number(flashArg) != 0;
   if(flashyThingy.initialized)
   {
      if(_loc1_)
      {
         flashyThingy.Start();
      }
      else
      {
         flashyThingy.Stop();
      }
   }
   else
   {
      flashyThingy.shouldPlay = _loc1_;
   }
}
function SetHighlighted(highlightArg)
{
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
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function onUnload()
{
   highlightClip.Close();
   delete SetState;
   delete OnPress;
   delete OnRelease;
   delete OnRollOver;
   delete OnRollOut;
   delete AttachContent;
   delete SetVisibility;
   delete SetEnabled;
   delete SetSelected;
   delete SetFlashing;
   delete SetHighlighted;
   delete GetHelpSite;
   delete onUnload;
}
if(!initialized)
{
   stateLabel = "_disabled";
   isEnabled = false;
   isSelected = false;
   isOver = false;
   var buttonClip = buttonStage.attachMovie(buttonClipName,"clip",0);
   buttonClip.client = this;
   buttonClip.gotoAndPlay("_disabled");
   AttachContent();
   SetEnabled("1");
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame && flashyThingy != undefined)
   {
   }
}
stop();
