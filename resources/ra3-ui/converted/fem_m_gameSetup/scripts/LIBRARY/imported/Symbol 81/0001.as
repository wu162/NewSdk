#initclip
class fem_m_gameSetup extends GameSetupBase
{
   var initPlayButton;
   var initReadyControls;
   var m_isClanGame;
   var m_isHost;
   var m_playerSlots;
   var m_playerStartPosMCArray;
   var m_screenButtons;
   var onShowMapPanel;
   var onShowRulesPanel;
   var refreshMapList;
   var refreshReadyControls;
   var refreshStartPositionControls;
   var registerIntroOuttroComponents;
   var setBroadcastCheckboxVisibility;
   var startIntro;
   function fem_m_gameSetup(screen)
   {
      super(screen);
   }
   function frameDelayedInit()
   {
      fscommand("CallGameFunction","%SetGameSetupMode?Mode=Online");
      var _loc5_ = new Object();
      loadVariables("QueryGameEngine?IS_CLAN_GAME",_loc5_);
      this.m_isClanGame = _loc5_.IS_CLAN_GAME == "1";
      if(this.m_isClanGame)
      {
         Cafe2_BaseUIScreen.m_screen.playerSlotBg.personality.text = "$HeaderClanName";
      }
      super.frameDelayedInit();
      this.initVoiceIPControls();
      this.initBroadcastOption();
      this.initReadyControls();
      this.initPlayButton();
      var _loc4_;
      if(this.m_isClanGame)
      {
         _loc4_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel;
         _loc4_.gameSpeedSlider.disable();
         _loc4_.resourcesDropdown.disable();
         _loc4_.cratesCheckbox.disable();
         _loc4_.defaultRules.disable();
      }
      if(this.m_isHost)
      {
         Cafe2_BaseUIScreen.m_screen.randomMap.show();
         Cafe2_BaseUIScreen.m_screen.randomMap.enable();
         Cafe2_BaseUIScreen.m_screen.randomMap.setOnMouseUpFunction(_global.bind0(this,this.onRandomMap));
         this.m_screenButtons.push(Cafe2_BaseUIScreen.m_screen.randomMap);
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.randomMap.hide();
      }
      this.refreshComponents();
      this.registerIntroOuttroComponents(this.m_screenButtons);
      this.startIntro();
   }
   function initNavigationButtons(navButtons)
   {
      trace("fem_m_gameSetup::initNavigationButtons() ");
      super.initNavigationButtons(navButtons);
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.enable();
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.show();
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.setOnMouseUpFunction(_global.bind0(this,this.onShowMapPanel));
      navButtons.push(Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton);
      if(extern.RA3PublicBeta == "1")
      {
         Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.disable();
         Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.hide();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.enable();
         Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.show();
         Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.setOnMouseUpFunction(_global.bind0(this,this.onShowRulesPanel));
      }
      navButtons.push(Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton);
   }
   function onScreenExit()
   {
      trace("fem_m_gameSetup::onScreenExit()");
      super.onScreenExit();
      delete _global.fem_m_gameSetup;
   }
   function localMessageHandler(messageCode)
   {
      var _loc4_ = false;
      switch(messageCode)
      {
         case _global.MSGCODE.FE_MP_ONLINE_GAME_COUNTDOWN_START:
            if(!this.m_isHost)
            {
               this.ConfigureClientForCountdown(true);
            }
            break;
         case _global.MSGCODE.FE_MP_ONLINE_GAME_COUNTDOWN_STOP:
            this.enableGameSetupUI();
            break;
         case _global.MSGCODE.FE_MP_COUNTDOWN_PAST_POINT_OF_NO_RETURN:
            this.disableGameSetupUI();
            break;
         default:
            _loc4_ = super.localMessageHandler(messageCode);
      }
      return _loc4_;
   }
   function disableGameSetupUI()
   {
      var _loc5_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel;
      _loc5_.gameSpeedSlider.disable();
      _loc5_.resourcesDropdown.disable();
      _loc5_.cratesCheckbox.disable();
      _loc5_.defaultRules.disable();
      _loc5_.commentaryCheckbox.disable();
      _loc5_.enableVoipCheckbox.disable();
      _loc5_.broadcastCheckbox.disable();
      Cafe2_BaseUIScreen.m_screen.randomMap.disable();
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapList.disable();
      var _loc4_ = 0;
      var _loc2_;
      while(_loc4_ < this.m_playerSlots.length)
      {
         _loc2_ = this.m_playerSlots[_loc4_];
         _loc2_.playerSlotOptions.disable();
         _loc2_.personalityOptions.disable();
         _loc2_.factionOptions.disable();
         _loc2_.teamOptions.disable();
         _loc2_.colorOptions.disable();
         _loc4_ = _loc4_ + 1;
      }
      var _loc3_ = 0;
      while(_loc3_ < this.m_playerStartPosMCArray.length)
      {
         this.m_playerStartPosMCArray[_loc3_].disable();
         _loc3_ = _loc3_ + 1;
      }
      Cafe2_BaseUIScreen.m_screen.playButton.disable();
   }
   function enableGameSetupUI()
   {
      var _loc5_;
      var _loc2_;
      var _loc3_;
      var _loc4_;
      if(this.m_isHost)
      {
         if(!this.m_isClanGame)
         {
            _loc5_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel;
            _loc5_.gameSpeedSlider.enable();
            _loc5_.resourcesDropdown.enable();
            _loc5_.cratesCheckbox.enable();
            _loc5_.defaultRules.enable();
            _loc5_.commentaryCheckbox.enable();
            _loc5_.enableVoipCheckbox.enable();
            _loc5_.broadcastCheckbox.enable();
         }
         Cafe2_BaseUIScreen.m_screen.randomMap.enable();
         Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapList.enable();
         _loc2_ = 0;
         while(_loc2_ < this.m_playerStartPosMCArray.length)
         {
            this.m_playerStartPosMCArray[_loc2_].enable();
            _loc2_ = _loc2_ + 1;
         }
         this.m_playerSlots[0].factionOptions.enable();
         this.m_playerSlots[0].teamOptions.enable();
         this.m_playerSlots[0].colorOptions.enable();
         _loc3_ = 1;
         while(_loc3_ < this.m_playerSlots.length)
         {
            _loc4_ = this.m_playerSlots[_loc3_];
            _loc4_.playerSlotOptions.enable();
            _loc3_ = _loc3_ + 1;
         }
         Cafe2_BaseUIScreen.m_screen.playButton.enable();
      }
      else
      {
         this.ConfigureClientForCountdown(false);
      }
   }
   function initVoiceIPControls()
   {
      var _loc3_ = 0;
      var _loc4_;
      while(_loc3_ < this.m_playerSlots.length)
      {
         _loc4_ = this.m_playerSlots[_loc3_].voipCheckBox;
         _loc4_.disable();
         _loc4_.setOnMouseDownFunction(_global.bind1(this,this.onVoipSendToStatusChanged,_loc3_));
         _loc3_ = _loc3_ + 1;
      }
      _loc3_ = 0;
      var _loc5_;
      while(_loc3_ < this.m_playerSlots.length)
      {
         _loc5_ = this.m_playerSlots[_loc3_].muteCheckBox;
         _loc5_.disable();
         _loc5_.setOnMouseDownFunction(_global.bind1(this,onVoipMuteStatusChanged,_loc3_));
         _loc3_ = _loc3_ + 1;
      }
   }
   function initBroadcastOption()
   {
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel.broadcastCheckbox;
      this.setBroadcastCheckboxVisibility(true);
      _loc3_.setOnMouseDownFunction(_global.bind0(this,this.onToggleBroadcastStatus));
      var _loc4_ = new Object();
      loadVariables("QueryGameEngine?GAME_BROADCASTER",_loc4_);
      _loc4_.GAME_BROADCASTER_ENABLE != "1" ? _loc3_.disable() : _loc3_.enable();
      _loc4_.GAME_BROADCASTER_VALUE != "1" ? _loc3_.unCheck() : _loc3_.check();
      var _loc5_ = new Object();
      loadVariables("QueryGameEngine?DISABLE_BROADCAST_ON_MOD",_loc5_);
      if(_loc5_.DISABLE_BROADCAST_ON_MOD == 1)
      {
         this.setBroadcastCheckboxVisibility(false);
      }
   }
   function onVoipSendToStatusChanged(slotId)
   {
      fscommand("CallGameFunction","%ChangePlayerSendStatus?Slot=" + String(slotId));
   }
   function onToggleBroadcastStatus()
   {
      fscommand("CallGameFunction","%ToggleBroadcastGame");
   }
   function refreshComponents()
   {
      super.refreshComponents();
      this.refreshReadyControls();
      this.refreshVoiceIPControls();
      ConfigForCountdown();
   }
   function refreshAIPersonalityControls()
   {
      if(!this.m_isClanGame)
      {
         super.refreshAIPersonalityControls();
         return undefined;
      }
      var _loc3_ = 0;
      var _loc4_;
      var _loc6_;
      var _loc5_;
      while(_loc3_ < this.m_playerSlots.length)
      {
         _loc4_ = new Object();
         loadVariables("QueryGameEngine?GET_CLAN_NAME?Slot=" + String(_loc3_),_loc4_);
         if(_loc4_.GET_CLAN_NAME == undefined)
         {
            this.m_playerSlots[_loc3_].personalityOptions.clear();
         }
         else
         {
            _loc6_ = new Array();
            _loc6_.push(0);
            _loc5_ = new Array();
            _loc5_.push(_loc4_.GET_CLAN_NAME);
            this.m_playerSlots[_loc3_].personalityOptions.setData(_loc5_,_loc6_);
            this.m_playerSlots[_loc3_].personalityOptions.setSelectedIndex(0);
         }
         this.m_playerSlots[_loc3_].personalityOptions.disable();
         _loc3_ = _loc3_ + 1;
      }
   }
   function refreshVoiceIPControls()
   {
      trace("fem_m_gameSetup::refreshVoipSendToCheckBox()");
      var _loc3_ = this.m_playerSlots.length;
      var _loc2_ = 0;
      while(_loc2_ < _loc3_)
      {
         this.refreshVoipSendToCheckBox(this.m_playerSlots[_loc2_].voipCheckBox,_loc2_);
         this.refreshVoipMuteCheckBox(this.m_playerSlots[_loc2_].muteCheckBox,_loc2_);
         _loc2_ = _loc2_ + 1;
      }
   }
   function refreshVoipSendToCheckBox(checkBox, slotId)
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?VOIP_SEND_TO?Slot=" + String(slotId),_loc1_);
      _loc1_.VOIP_SEND_TO_ENABLED != "1" ? checkBox.disable() : checkBox.enable();
      _loc1_.VOIP_SEND_TO_VALUE != "1" ? checkBox.unCheck() : checkBox.check();
   }
   function refreshVoipMuteCheckBox(checkBox, slotId)
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?VOIP_MUTE?Slot=" + String(slotId),_loc1_);
      _loc1_.VOIP_MUTE_ENABLED != "1" ? checkBox.disable() : checkBox.enable();
      _loc1_.VOIP_MUTE_VALUE != "1" ? checkBox.unCheck() : checkBox.check();
   }
   function ConfigureClientForCountdown(inCountdown)
   {
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?LOCAL_PLAYER_SLOT",_loc3_);
      var _loc2_ = this.m_playerSlots[Number(_loc3_.LOCAL_PLAYER_SLOT)];
      if(inCountdown)
      {
         _loc2_.factionOptions.disable();
         _loc2_.teamOptions.disable();
         _loc2_.colorOptions.disable();
      }
      else
      {
         _loc2_.factionOptions.enable();
         _loc2_.teamOptions.enable();
         _loc2_.colorOptions.enable();
      }
   }
   function onRandomMap()
   {
      fscommand("CallGameFunction","%SetRandomMap");
      this.refreshStartPositionControls();
      this.refreshMapList();
   }
}
#endinitclip
