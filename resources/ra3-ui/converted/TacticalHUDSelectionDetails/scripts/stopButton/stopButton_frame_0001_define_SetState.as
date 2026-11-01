function SetState(stateLabel)
{
   imageStage.clip.gotoAndPlay(stateLabel);
   backgroundStage.clip.gotoAndPlay(stateLabel);
   currentStateLabel = stateLabel;
}
function SetEnabled(enableArg)
{
   var _loc1_ = Number(enableArg) != 0;
   if(_loc1_ != isEnabled)
   {
      isEnabled = _loc1_;
      currentStateLabel = !isEnabled ? "_disabled" : "_up";
      imageStage.clip.gotoAndPlay(currentStateLabel);
      backgroundStage.clip.gotoAndPlay(currentStateLabel);
   }
}
function ReattachContent(stageClip, prefix)
{
   trace("stageClip >> " + stageClip + " : prefix >> " + prefix);
   if(prefix == undefined || prefix.length <= 0)
   {
      return undefined;
   }
   var _loc1_ = stageClip.clip;
   if(_loc1_ != undefined)
   {
      _loc1_.removeMovieClip();
   }
   var _loc3_ = prefix + (!isSelected ? "Normal" : "Highlighted");
   _loc1_ = stageClip.attachMovie(_loc3_,"clip",0);
   _loc1_.gotoAndPlay(currentStateLabel);
}
function SetSelected(selectedArg)
{
   var _loc1_ = Number(selectedArg) != 0;
   if(_loc1_ != isSelected)
   {
      isSelected = _loc1_;
      ReattachContent(imageStage,imageClip);
      ReattachContent(backgroundStage,backgroundClip);
   }
}
function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   _visible = _loc1_;
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
   delete SetEnabled;
   delete ReattachContent;
   delete SetSelected;
   delete SetVisibility;
   delete SetHighlighted;
   delete GetHelpSite;
   delete onUnload;
}
if(!initialized)
{
   if(client == undefined)
   {
      client = _parent;
   }
   currentStateLabel = "_disabled";
   isEnabled = false;
   isSelected = false;
   SetEnabled(1);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      SetEnabled(0);
   }
}
