function Hide()
{
   letterBoxLayer._visible = false;
   if(hideMainCount++ == 0)
   {
      main._visible = false;
      upperSidebarLayer._visible = false;
   }
   selectionFeedbackSurface._visible = false;
   floatingTextSurface._visible = false;
}
function Show()
{
   letterBoxLayer._visible = true;
   if(--hideMainCount == 0)
   {
      main._visible = true;
      upperSidebarLayer._visible = true;
   }
   selectionFeedbackSurface._visible = true;
   floatingTextSurface._visible = true;
}
function FadeIn()
{
   gotoAndStop("_fadeIn");
   play();
   if(letterBoxVisible == false || showRadarDuringLetterBox == false)
   {
      upperSidebarLayer.gotoAndPlay("_fadeIn");
   }
   if(--hideMainCount == 0)
   {
      main._visible = true;
   }
}
function FadeOut()
{
   gotoAndStop("_fadeOut");
   play();
   if(letterBoxVisible == false || showRadarDuringLetterBox == false)
   {
      upperSidebarLayer.gotoAndPlay("_fadeOut");
   }
}
function HideShroud()
{
   shroud.gotoAndPlay("_fade");
}
function GetInputShield()
{
   return Pointer.encode(inputShield);
}
function GetHelpBox()
{
   return Pointer.encode(main.helpBox);
}
function GetSelectionFeedbackSurface()
{
   return Pointer.encode(selectionFeedbackSurface);
}
function GetRadar()
{
   return Pointer.encode(upperSidebarLayer.upperSidebar.radar);
}
function GetMainCommandBar()
{
   return Pointer.encode(upperSidebarLayer.upperSidebar.mainCommandBar);
}
function GetSelectionDetailsPanel()
{
   return Pointer.encode(main.lowerSidebar.selectionDetails);
}
function GetResourceDisplay()
{
   return Pointer.encode(upperSidebarLayer.upperSidebar.resourceDisplay);
}
function GetCommandModeBar()
{
   return Pointer.encode(upperSidebarLayer.upperSidebar.commandModeBar);
}
function GetEnergyMeter()
{
   return Pointer.encode(upperSidebarLayer.upperSidebar.energyMeter);
}
function GetPlayerPowersBar()
{
   return Pointer.encode(main.playerPowersBar);
}
function GetSpecialPowerTimerDisplay()
{
   return Pointer.encode(main.specialPowerTimerDisplay);
}
function GetDialogStage()
{
   return Pointer.encode(dialogStage);
}
function GetCutSceneStage()
{
   return Pointer.encode(cutSceneStage);
}
function GetObjectiveIndicatorStage()
{
   return Pointer.encode(main.objectiveIndicatorStage);
}
function GetFloatingTextSurface()
{
   return Pointer.encode(floatingTextSurface);
}
function GetMessageSurface()
{
   return Pointer.encode(main.messageSurface);
}
function GetTimerSurface()
{
   return Pointer.encode(main.timerSurface);
}
function GetPopUpMetaStage()
{
   return Pointer.encode(main.popUpMetaStage);
}
function GetObserverControlPanel()
{
   return Pointer.encode(main.observerControl);
}
function GetTelestratorControlPanel()
{
   return Pointer.encode(main.telestratorControl);
}
function GetReplayControlPanel()
{
   return Pointer.encode(main.replayControl);
}
function GetVoiceChatControlPanel()
{
   return Pointer.encode(main.voiceChatControl);
}
function GetTickerPanel()
{
   return Pointer.encode(main.ticker);
}
function GetTelestratorSurface()
{
   return Pointer.encode(main.telestratorSurface);
}
function GetSubtitlePanelSurface()
{
   return Pointer.encode(subtitlePanelSurface);
}
function GetCoOpAIOrderPanel()
{
   return Pointer.encode(main.coOpAIOrderPanel);
}
function GetBeaconStage()
{
   return Pointer.encode(beaconStage);
}
function GetFadeSurface()
{
   return Pointer.encode(fadeSurface);
}
function GetDrawableOverlayStage()
{
   return Pointer.encode(drawableOverlayStage);
}
function GetPlayerExperiencePanel()
{
   return Pointer.encode(main.expPanel);
}
function SetDisplayRegion(topArg, leftArg, bottomArg, rightArg)
{
   var _loc4_ = Number(topArg);
   var _loc5_ = Number(leftArg);
   var _loc6_ = Number(bottomArg);
   var _loc7_ = Number(rightArg);
   var _loc11_ = _loc7_ - _loc5_;
   var _loc12_ = _loc6_ - _loc4_;
   var _loc9_ = (_loc5_ + _loc7_) * 0.5;
   var _loc8_ = (_loc4_ + _loc6_) * 0.5;
   _root.displayLeft = _loc5_;
   _root.displayTop = _loc4_;
   _root.displayWidth = _loc11_;
   _root.displayHeight = _loc12_;
   var _loc3_ = 0;
   var _loc2_;
   while(_loc3_ < leftAnchoredClips.length)
   {
      _loc2_ = leftAnchoredClips[_loc3_];
      _loc2_._x = GlobalToLocalX(_loc2_._parent,_loc5_) + leftMargins[_loc3_];
      _loc3_ = _loc3_ + 1;
   }
   _loc3_ = 0;
   while(_loc3_ < rightAnchoredClips.length)
   {
      _loc2_ = rightAnchoredClips[_loc3_];
      _loc2_._x = GlobalToLocalX(_loc2_._parent,_loc7_) - rightMargins[_loc3_];
      _loc3_ = _loc3_ + 1;
   }
   _loc3_ = 0;
   while(_loc3_ < topAnchoredClips.length)
   {
      _loc2_ = topAnchoredClips[_loc3_];
      _loc2_._y = GlobalToLocalY(_loc2_._parent,_loc4_) + topMargins[_loc3_];
      _loc3_ = _loc3_ + 1;
   }
   _loc3_ = 0;
   while(_loc3_ < bottomAnchoredClips.length)
   {
      _loc2_ = bottomAnchoredClips[_loc3_];
      _loc2_._y = GlobalToLocalY(_loc2_._parent,_loc6_) - bottomMargins[_loc3_];
      _loc3_ = _loc3_ + 1;
   }
   _loc3_ = 0;
   while(_loc3_ < centerAnchoredClips.length)
   {
      _loc2_ = centerAnchoredClips[_loc3_];
      _loc2_._x = GlobalToLocalX(_loc2_._parent,_loc9_) + centerOffsets[_loc3_].x;
      _loc2_._y = GlobalToLocalY(_loc2_._parent,_loc8_) + centerOffsets[_loc3_].y;
      _loc3_ = _loc3_ + 1;
   }
   _loc3_ = 0;
   while(_loc3_ < fullScreenClips.length)
   {
      _loc2_ = fullScreenClips[_loc3_];
      _loc2_._x = GlobalToLocalX(_loc2_._parent,_loc5_);
      _loc2_._width = GlobalToLocalX(_loc2_._parent,_loc7_) - _loc2_._x;
      _loc2_._y = GlobalToLocalY(_loc2_._parent,_loc4_);
      _loc2_._height = GlobalToLocalY(_loc2_._parent,_loc6_) - _loc2_._y;
      _loc3_ = _loc3_ + 1;
   }
   var _loc10_ = 5;
   inputShield._x = _loc5_ - _loc10_;
   inputShield._width = _loc11_ + _loc10_ * 2;
   inputShield._y = _loc4_ - _loc10_;
   inputShield._height = _loc12_ + _loc10_ * 2;
}
function SetLetterBoxVisibility(visArg, showRadarArg)
{
   var _loc2_ = Number(visArg) != 0;
   var _loc1_ = Number(showRadarArg) != 0;
   if(_loc2_)
   {
      if(!letterBoxVisible)
      {
         letterBoxVisible = true;
         letterBoxLayer.gotoAndPlay("_fadeIn");
      }
      trace("showRadarDuringLetterBox=" + String(showRadarDuringLetterBox));
      trace("showRadar=" + String(_loc1_));
      if(!showRadarDuringLetterBox && _loc1_)
      {
         trace("fading in radar");
         upperSidebarLayer.gotoAndPlay("_fadeIn");
      }
      else if(showRadarDuringLetterBox && !_loc1_)
      {
         trace("fading out radar");
         upperSidebarLayer.gotoAndPlay("_fadeOut");
      }
      showRadarDuringLetterBox = _loc1_;
   }
   else if(letterBoxVisible)
   {
      letterBoxVisible = false;
      letterBoxLayer.gotoAndPlay("_fadeOut");
   }
}
function SetCurrentFaction(faction)
{
   var _loc2_;
   if(faction != _root.currentFaction)
   {
      _root.currentFaction = faction;
      for(var _loc4_ in _root.factionObserverMap)
      {
         _loc2_ = _root.factionObserverMap[_loc4_];
         _loc2_.OnCurrentFactionChanged(faction);
      }
   }
}
function GlobalToLocalX(clip, x)
{
   var _loc4_ = clip._root;
   var _loc2_ = 0;
   var _loc3_ = 1;
   ancestor = clip;
   var _loc1_;
   while(ancestor != _loc4_)
   {
      _loc1_ = ancestor._xscale * 0.009999999776482582;
      _loc2_ *= _loc1_;
      _loc2_ -= ancestor._x;
      _loc3_ /= _loc1_;
      ancestor = ancestor._parent;
   }
   return (x + _loc2_) * _loc3_;
}
function GlobalToLocalY(clip, y)
{
   var _loc4_ = clip._root;
   var _loc2_ = 0;
   var _loc3_ = 1;
   ancestor = clip;
   var _loc1_;
   while(ancestor != _loc4_)
   {
      _loc1_ = ancestor._yscale * 0.009999999776482582;
      _loc2_ *= _loc1_;
      _loc2_ -= ancestor._y;
      _loc3_ /= _loc1_;
      ancestor = ancestor._parent;
   }
   return (y + _loc2_) * _loc3_;
}
function onUnloading()
{
   trace("TacticalHUD.swf onUnloading()");
   gadgetFlashStage.onUnloading();
}
function onUnload()
{
   trace("TacticalHUD.swf onUnload()");
   delete Hide;
   delete Show;
   delete FadeIn;
   delete FadeOut;
   delete HideShroud;
   delete GetInputShield;
   delete GetHelpBox;
   delete GetSelectionFeedbackSurface;
   delete GetRadar;
   delete GetMainCommandBar;
   delete GetSelectionDetailsPanel;
   delete GetResourceDisplay;
   delete GetCommandModeBar;
   delete GetEnergyMeter;
   delete GetPlayerPowerBar;
   delete GetPlayerPowerTimerDisplay;
   delete GetDialogStage;
   delete GetCutSceneStage;
   delete GetObjectiveIndicatorStage;
   delete GetFloatingTextSurface;
   delete GetMessageSurface;
   delete GetTimerSurface;
   delete GetPopUpMetaStage;
   delete GetObserverControlPanel;
   delete GetTelestratorControlPanel;
   delete GetReplayControlPanel;
   delete GetVoiceChatControlPanel;
   delete GetTickerPanel;
   delete GetTelestratorSurface;
   delete GetSubtitlePanelSurface;
   delete GetCoOpAIOrderPanel;
   delete GetBeaconStage;
   delete GetDrawableOverlayStage;
   delete GetPlayerExperiencePanel;
   delete SetDisplayRegion;
   delete SetLetterBoxVisibility;
   delete SetCurrentFaction;
   delete GlobalToLocalX;
   delete GlobalToLocalY;
   delete onUnloading;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   beaconStage.loadMovie("TacticalHUDBeaconStage.swf");
   drawableOverlayStage.loadMovie("ig_DrawableOverlayStage.swf");
   gadgetFlashStage.loadMovie("igm_gadgetFlashStage.swf");
   hideMainCount = 1;
   main._visible = false;
   letterBoxVisible = false;
   showRadarDuringLetterBox = false;
   leftAnchoredClips = [main.timerSurface,main.messageSurface,main.playerPowersBar,main.specialPowerTimerDisplay,main.ticker,main.coOpAIOrderPanel,main.expPanel,watermark];
   rightAnchoredClips = [main.replayControl,main.telestratorControl,main.observerControl,main.voiceChatControl,upperSidebarLayer,main.lowerSidebar];
   topAnchoredClips = [main.timerSurface,main.messageSurface,main.playerPowersBar,main.replayControl,main.telestratorControl,main.observerControl,main.voiceChatControl,main.coOpAIOrderPanel,upperSidebarLayer];
   bottomAnchoredClips = [main.ticker,main.lowerSidebar,main.expPanel,main.specialPowerTimerDisplay,subtitlePanelSurface,watermark];
   centerAnchoredClips = [dialogStage,cutSceneStage,main.popUpMetaStage,main.objectiveIndicatorStage];
   fullScreenClips = [shroud,letterBoxLayer,main.telestratorSurface,selectionFeedbackSurface,floatingTextSurface,fadeSurface];
   leftMargins = new Array(leftAnchoredClips.length);
   var i = 0;
   while(i < leftAnchoredClips.length)
   {
      var clip = leftAnchoredClips[i];
      leftMargins[i] = clip._x - GlobalToLocalX(clip._parent,0);
      i++;
   }
   rightMargins = new Array(rightAnchoredClips.length);
   var i = 0;
   while(i < rightAnchoredClips.length)
   {
      var clip = rightAnchoredClips[i];
      rightMargins[i] = GlobalToLocalX(clip._parent,1365.300048828125) - clip._x;
      i++;
   }
   topMargins = new Array(topAnchoredClips.length);
   var i = 0;
   while(i < topAnchoredClips.length)
   {
      var clip = topAnchoredClips[i];
      topMargins[i] = clip._y - GlobalToLocalY(clip._parent,0);
      i++;
   }
   bottomMargins = new Array(bottomAnchoredClips.length);
   var i = 0;
   while(i < bottomAnchoredClips.length)
   {
      var clip = bottomAnchoredClips[i];
      bottomMargins[i] = GlobalToLocalY(clip._parent,768) - clip._y;
      i++;
   }
   centerOffsets = new Array(centerAnchoredClips.length);
   var i = 0;
   while(i < centerAnchoredClips.length)
   {
      var clip = centerAnchoredClips[i];
      centerOffsets[i] = new Object();
      centerOffsets[i].x = clip._x - GlobalToLocalX(clip._parent,682.6500244140625);
      centerOffsets[i].y = clip._y - GlobalToLocalY(clip._parent,384);
      i++;
   }
   _root.displayLeft = 0;
   _root.displayTop = 0;
   _root.displayWidth = 1365.300048828125;
   _root.displayHeight = 768;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      shroud._visible = false;
      SetDisplayRegion(0,0,768,1024);
      SetCurrentFaction("Soviet");
      FadeIn();
   }
}
stop();
