#initclip
class GameSetupBase extends Cafe2_BaseUIScreen implements IListboxEventListener
{
   var m_isHost;
   var m_localMessageDispatcher;
   var m_mapIds;
   var m_playerSlots;
   var m_playerStartPosMCArray;
   var m_screenButtons;
   function GameSetupBase(screen)
   {
      super(screen);
      trace("GameSetupBase::GameSetupBase()");
      Cafe2_BaseUIScreen.m_thisClass = this;
      this.m_localMessageDispatcher = _global.bind1DynamicParams(this,this.localMessageHandler);
      this.m_mapIds = new Array();
      this.m_screenButtons = new Array();
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
   }
   function frameDelayedInit()
   {
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?IsPcGameHost",_loc3_);
      this.m_isHost = _loc3_.IsPcGameHost == "1";
      this.initGUI();
      _global.gMH.addMessageHandler(this.m_localMessageDispatcher);
   }
   function OnListboxIndexMouseClick(index, previouslySelectedIndex)
   {
      this.OnMapListSelectionChanged(index);
   }
   function OnListboxIndexElementMouseClick(index, element, previouslySelectedIndex)
   {
      this.OnMapListSelectionChanged(index);
   }
   function initGUI()
   {
      trace("GameSetupBase::initGUI() ");
      this.initNavigationButtons(this.m_screenButtons);
      this.initMapPanel();
      this.initRulesPanel(this.m_screenButtons);
      this.initPlayerSlots();
   }
   function initNavigationButtons(navButtons)
   {
      trace("GameSetupBase::initNavigationButtons() ");
      if(this.m_isHost)
      {
         Cafe2_BaseUIScreen.m_screen.playButton.show();
         Cafe2_BaseUIScreen.m_screen.playButton.enable();
         Cafe2_BaseUIScreen.m_screen.playButton.setOnMouseUpFunction(_global.bind0(this,this.playGame));
         navButtons.push(Cafe2_BaseUIScreen.m_screen.playButton);
      }
      Cafe2_BaseUIScreen.m_screen.backButton.show();
      Cafe2_BaseUIScreen.m_screen.backButton.enable();
      Cafe2_BaseUIScreen.m_screen.backButton.setOnMouseUpFunction(_global.bind0(this,this.backButtonClicked));
      navButtons.push(Cafe2_BaseUIScreen.m_screen.backButton);
   }
   function initMapPanel()
   {
      trace("GameSetupBase::initMapPanel() Map panel movieclip: " + Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel);
      var _loc4_ = Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel;
      this.m_playerStartPosMCArray = [_loc4_.startPosition0,_loc4_.startPosition1,_loc4_.startPosition2,_loc4_.startPosition3,_loc4_.startPosition4,_loc4_.startPosition5];
      var _loc3_ = 0;
      while(_loc3_ < this.m_playerStartPosMCArray.length)
      {
         this.m_playerStartPosMCArray[_loc3_].hide();
         this.m_playerStartPosMCArray[_loc3_].disable();
         this.m_playerStartPosMCArray[_loc3_].setOnMouseUpFunction(_global.bind1(this,this.onStartPositionClicked,_loc3_));
         _loc3_ = _loc3_ + 1;
      }
      _loc4_.campaignCheckbox.show();
      _loc4_.campaignCheckbox.enable();
      _loc4_.campaignCheckbox.setOnMouseDownFunction(_global.bind0(this,this.onMapTypeChanged));
      this.createMapSelectionScrollList();
   }
   function initRulesPanel(buttons)
   {
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel;
      trace("GameSetupBase::initRulesPanel() Rules panel movieclip: " + Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel);
      var _loc4_;
      if(extern.RA3PublicBeta == "1")
      {
         _loc3_._visible = false;
      }
      else
      {
         _loc3_.gameSpeedSlider.show();
         _loc3_.resourcesDropdown.show();
         _loc3_.cratesCheckbox.show();
         _loc3_.commentaryCheckbox.show();
         _loc3_.enableVoipCheckbox.show();
         this.setBroadcastCheckboxVisibility(false);
         if(this.m_isHost)
         {
            _loc3_.gameSpeedSlider.enable();
            _loc3_.resourcesDropdown.enable();
            _loc3_.cratesCheckbox.enable();
            _loc3_.commentaryCheckbox.enable();
            _loc3_.enableVoipCheckbox.enable();
            _loc3_.gameSpeedSlider.setOnChange(_global.bind2DynamicParams(this,this.onGameSpeedChanged));
            _loc3_.resourcesDropdown.setOnChange(_global.bind1DynamicParams(this,this.onResourcesChanged));
            _loc3_.cratesCheckbox.setOnMouseDownFunction(_global.bind0(this,this.onCratesChanged));
            _loc3_.commentaryCheckbox.setOnMouseDownFunction(_global.bind0(this,this.onCommentaryChanged));
            _loc3_.enableVoipCheckbox.setOnMouseDownFunction(_global.bind0(this,this.onVoipChanged));
            _loc4_ = _loc3_.defaultRules;
            _loc4_.show();
            _loc4_.enable();
            _loc4_.setOnMouseUpFunction(_global.bind0(this,this.onResetRules));
            buttons.push(_loc4_);
         }
         else
         {
            _loc3_.gameSpeedSlider.disable();
            _loc3_.resourcesDropdown.disable();
            _loc3_.cratesCheckbox.disable();
            _loc3_.commentaryCheckbox.disable();
            _loc3_.enableVoipCheckbox.disable();
            _loc3_.defaultRules.hide();
            _loc3_.defaultRules.disable();
         }
      }
   }
   function initPlayerSlots()
   {
      trace("GameSetupBase::initPlayerSlots() ");
      this.m_playerSlots = [Cafe2_BaseUIScreen.m_screen.slotMC_1,Cafe2_BaseUIScreen.m_screen.slotMC_2,Cafe2_BaseUIScreen.m_screen.slotMC_3,Cafe2_BaseUIScreen.m_screen.slotMC_4,Cafe2_BaseUIScreen.m_screen.slotMC_5,Cafe2_BaseUIScreen.m_screen.slotMC_6];
      var _loc3_ = 0;
      while(_loc3_ < this.m_playerSlots.length)
      {
         this.m_playerSlots[_loc3_].slotId = _loc3_;
         this.m_playerSlots[_loc3_].playerSlotOptions.show();
         this.m_playerSlots[_loc3_].playerSlotOptions.disable();
         this.m_playerSlots[_loc3_].playerSlotOptions.setOnChange(_global.bind1DynamicParams(this,this.onPlayerStatusChanged));
         this.m_playerSlots[_loc3_].factionOptions.show();
         this.m_playerSlots[_loc3_].factionOptions.setOnChange(_global.bind1DynamicParams(this,this.onFactionChanged));
         this.m_playerSlots[_loc3_].personalityOptions.show();
         this.m_playerSlots[_loc3_].personalityOptions.setOnChange(_global.bind1DynamicParams(this,this.onAIPersonalityChanged));
         this.m_playerSlots[_loc3_].teamOptions.show();
         this.m_playerSlots[_loc3_].teamOptions.setOnChange(_global.bind1DynamicParams(this,this.onTeamChanged));
         this.m_playerSlots[_loc3_].colorOptions.show();
         this.m_playerSlots[_loc3_].colorOptions.setOnChange(_global.bind1DynamicParams(this,this.onColorChanged));
         _loc3_ = _loc3_ + 1;
      }
   }
   function initReadyControls()
   {
      var _loc3_ = 0;
      var _loc4_;
      while(_loc3_ < this.m_playerSlots.length)
      {
         _loc4_ = this.m_playerSlots[_loc3_].readyCheckBox;
         _loc4_.show();
         _loc4_.disable();
         _loc4_.setOnMouseDownFunction(_global.bind0(this,this.onReadyStatusChanged));
         _loc3_ = _loc3_ + 1;
      }
   }
   function initPlayButton()
   {
      if(this.m_isHost)
      {
         Cafe2_BaseUIScreen.m_screen.playButton.show();
         Cafe2_BaseUIScreen.m_screen.playButton.enable();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.playButton.hide();
         Cafe2_BaseUIScreen.m_screen.playButton.disable();
      }
   }
   function setBroadcastCheckboxVisibility(isVisible)
   {
      var _loc1_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel;
      if(isVisible)
      {
         _loc1_.broadcastCheckbox.show();
         _loc1_.broadcastCheckboxLabel._visible = true;
         _loc1_.broadcastCheckboxShadowLabel._visible = true;
      }
      else
      {
         _loc1_.broadcastCheckbox.hide();
         _loc1_.broadcastCheckboxLabel._visible = false;
         _loc1_.broadcastCheckboxShadowLabel._visible = false;
      }
   }
   function createMapSelectionScrollList()
   {
      var _loc4_ = Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapListScrollbar;
      _loc4_.show();
      _loc4_.enable();
      var _loc2_ = Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapList;
      _loc2_.addListener(this);
      var _loc3_ = new Array();
      _loc3_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:270,leftMargin:0,rightMargin:0});
      _loc3_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:75,leftMargin:26,rightMargin:0});
      _loc2_.init(_loc3_,26);
      _loc2_.attachScrollbarController(_loc4_);
      _loc2_.show();
      _loc2_.enable();
      _loc2_.moveViewToFirst();
      _loc2_.refreshDisplay();
      _loc2_.refreshScrollbar();
   }
   function localMessageHandler(messageCode)
   {
      var _loc3_ = false;
      if(messageCode === _global.MSGCODE.FE_MP_UPDATE_GAME_SETTINGS)
      {
         this.refreshComponents();
         _loc3_ = true;
      }
      return _loc3_;
   }
   function onScreenExit()
   {
      trace("GameSetupBase::onScreenExit()");
      fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
      _global.gMH.removeMessageHandler(this.m_localMessageDispatcher);
      delete this.m_localMessageDispatcher;
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapList;
      _loc3_.removeListener(this);
      _loc3_.shutdown();
      this.clearData();
      delete _global.GameSetupBase;
      delete _global.Cafe2_BaseUIScreen;
   }
   function playGame()
   {
      trace("GameSetupBase::playGame()");
      fscommand("CallGameFunction","%StartGame");
   }
   function onMapTypeChanged()
   {
      trace("GameSetupBase::onMapTypeChanged() " + Cafe2_BaseUIScreen.m_screen);
      var _loc2_ = !Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.campaignCheckbox.isChecked() ? "0" : "1";
      fscommand("CallGameFunction","%SetMapType?MapType=" + _loc2_);
      if(_loc2_)
      {
         _global.GAME.MODE = _global.ENUM.GAME_MODE_CAMPAIGN;
         trace("GameSetupBase::onMapTypeChanged()- Game mode set to GAME_MODE_CAMPAIGN.");
      }
      else
      {
         _global.GAME.MODE = _global.ENUM.GAME_MODE_SKIRMISH;
         trace("GameSetupBase::onMapTypeChanged()- Game mode set to GAME_MODE_SKIRMISH.");
      }
   }
   function OnMapListSelectionChanged(index)
   {
      trace("GameSetupBase::OnMapListSelectionChanged(), index is " + index);
      fscommand("CallGameFunction","%SetMap?Map=" + this.m_mapIds[index]);
   }
   function onShowMapPanel()
   {
      trace("GameSetupBase::onShowMapPanel()");
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel._visible = true;
      Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel._visible = false;
   }
   function onShowRulesPanel()
   {
      trace("GameSetupBase::onShowRulesPanel()");
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel._visible = false;
      Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel._visible = true;
   }
   function onPlayerStatusChanged(playerStatusMC)
   {
      var _loc2_ = String(playerStatusMC.getValueAtIndex(playerStatusMC.getCurrentIndex()));
      var _loc1_ = String(playerStatusMC._parent.slotId);
      fscommand("CallGameFunction","%SetPlayerStatus?Slot=" + _loc1_ + "|Status=" + _loc2_);
   }
   function onFactionChanged(factionsMC)
   {
      var _loc3_ = String(factionsMC.getValueAtIndex(factionsMC.getCurrentIndex()));
      var _loc2_ = String(factionsMC._parent.slotId);
      fscommand("CallGameFunction","%SetPlayerTemplate?Slot=" + _loc2_ + "|Template=" + _loc3_);
   }
   function onTeamChanged(teamsMC)
   {
      var _loc3_ = String(teamsMC.getValueAtIndex(teamsMC.getCurrentIndex()));
      var _loc1_ = String(teamsMC._parent.slotId);
      fscommand("CallGameFunction","%SetTeam?Slot=" + _loc1_ + "|Team=" + _loc3_);
   }
   function onColorChanged(colorsMC)
   {
      var _loc2_ = String(colorsMC.getValueAtIndex(colorsMC.getCurrentIndex()));
      var _loc1_ = String(colorsMC._parent.slotId);
      fscommand("CallGameFunction","%SetColor?Slot=" + _loc1_ + "|Color=" + _loc2_);
   }
   function onAIPersonalityChanged(personalitiesMC)
   {
      var _loc3_ = String(personalitiesMC.getValueAtIndex(personalitiesMC.getCurrentIndex()));
      var _loc1_ = String(personalitiesMC._parent.slotId);
      fscommand("CallGameFunction","%SetAIPersonality?Slot=" + _loc1_ + "|Personality=" + _loc3_);
   }
   function onReadyStatusChanged()
   {
      trace("GameSetupBase::onReadyStatusChanged()");
      fscommand("CallGameFunction","%ToggleReadyStatus");
   }
   function onStartPositionClicked(startPosition)
   {
      trace("GameSetupBase::onStartPositionClicked() Position: " + startPosition);
      fscommand("CallGameFunction","%CycleStartPositionOwner?Position=" + startPosition);
   }
   function onGameSpeedChanged(component, newSpeed)
   {
      trace("GameSetupBase::onGameSpeedChanged()");
      fscommand("CallGameFunction","%SetGameSpeed?GameSpeed=" + String(newSpeed));
   }
   function onResourcesChanged(resourcesMC)
   {
      trace("GameSetupBase::onResourcesChanged()");
      var _loc1_ = String(resourcesMC.getValueAtIndex(resourcesMC.getCurrentIndex()));
      fscommand("CallGameFunction","%SetInitialResources?Resources=" + _loc1_);
   }
   function onVoipChanged()
   {
      trace("GameSetupBase::onVoipChanged()");
      fscommand("CallGameFunction","%ToggleVoipRule");
   }
   function onCratesChanged()
   {
      trace("GameSetupBase::onCratesChanged()");
      fscommand("CallGameFunction","%ToggleRandomCrates");
   }
   function onCommentaryChanged()
   {
      trace("GameSetupBase::onCommentaryChanged()");
      fscommand("CallGameFunction","%ToggleCommentary");
   }
   function onResetRules()
   {
      trace("GameSetupBase::onResetRules()");
      fscommand("CallGameFunction","%ResetRules");
   }
   function refreshComponents()
   {
      trace("GameSetupBase::refreshComponents()");
      this.refreshMapList();
      this.refreshSlotStatusControls();
      this.refreshCampaignMapControls();
      this.refreshResourcesControl();
      this.refreshRulesCheckbox("RANDOM_CRATES",Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel.cratesCheckbox);
      this.refreshRulesCheckbox("ENABLE_VOIP",Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel.enableVoipCheckbox);
      this.refreshRulesCheckbox("ALLOW_COMMENTARY",Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel.commentaryCheckbox);
      this.refreshGameSpeedControls();
      this.refreshTeamControls();
      this.refreshFactionControls();
      this.refreshAIPersonalityControls();
      this.refreshStartPositionControls();
      this.refreshColorControls();
   }
   function refreshMapList()
   {
      trace("GameSetupBase::refreshMapList()");
      var _loc5_ = Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapList;
      var _loc6_ = new Object();
      var _loc4_ = new Array();
      var _loc3_ = new Array();
      loadVariables("QueryGameEngine?MP_MAP_LIST",_loc6_);
      this.m_mapIds = _loc6_.MP_MAP_LIST.split(",");
      var _loc7_ = Number(_loc6_.MP_MAP_LIST_SELECTED_INDEX);
      var _loc2_;
      if(this.m_isHost)
      {
         _loc2_ = 0;
         while(_loc2_ < this.m_mapIds.length)
         {
            _loc4_[_loc2_] = "$MP_MAP_LIST_" + String(_loc2_);
            _loc3_[_loc2_] = "$MP_MAP_SIZE_" + String(_loc2_);
            _loc2_ = _loc2_ + 1;
         }
      }
      else
      {
         _loc4_[0] = "$MP_MAP_LIST_0";
         _loc3_[0] = "$MP_MAP_SIZE_0";
      }
      _loc5_.setColumnData(0,_loc4_);
      _loc5_.setColumnData(1,_loc3_);
      _loc5_.setSelectedIndex(_loc7_);
      _loc5_.refreshDisplay();
      _loc5_.refreshScrollbar();
   }
   function refreshSlotStatusControls()
   {
      trace("GameSetupBase::refreshSlotStatusControls()");
      var _loc3_ = 0;
      var _loc5_;
      var _loc6_;
      var _loc4_;
      var _loc7_;
      var _loc2_;
      while(_loc3_ < this.m_playerSlots.length)
      {
         _loc5_ = new Object();
         _loc6_ = new Array();
         _loc4_ = new Array();
         _loc7_ = 0;
         loadVariables("QueryGameEngine?PLAYER_STATUS?Slot=" + String(_loc3_),_loc5_);
         _loc4_ = _loc5_.PLAYER_STATUS.split(",");
         _loc7_ = Number(_loc4_.shift());
         _loc2_ = 0;
         while(_loc2_ < _loc4_.length)
         {
            _loc6_[_loc2_] = "$PLAYER_STATUS_" + String(_loc3_) + "_" + String(_loc2_);
            _loc2_ = _loc2_ + 1;
         }
         this.enableControl(_loc4_.length > 1,this.m_playerSlots[_loc3_].playerSlotOptions);
         this.m_playerSlots[_loc3_].playerSlotOptions.setData(_loc6_,_loc4_);
         this.m_playerSlots[_loc3_].playerSlotOptions.setSelectedIndex(_loc7_);
         _loc3_ = _loc3_ + 1;
      }
   }
   function refreshTeamControls()
   {
      trace("GameSetupBase::refreshTeamControls()");
      var _loc4_ = 0;
      var _loc5_;
      var _loc6_;
      var _loc3_;
      var _loc7_;
      var _loc2_;
      while(_loc4_ < this.m_playerSlots.length)
      {
         _loc5_ = new Object();
         _loc6_ = new Array();
         _loc3_ = new Array();
         _loc7_ = 0;
         loadVariables("QueryGameEngine?TEAM_OPTIONS?Slot=" + String(_loc4_),_loc5_);
         _loc3_ = _loc5_.TEAM_OPTIONS_VALUES.split(",");
         _loc7_ = Number(_loc3_.shift());
         _loc2_ = 0;
         while(_loc2_ < _loc3_.length)
         {
            _loc6_[_loc2_] = "$TEAM_" + String(_loc3_[_loc2_]);
            _loc2_ = _loc2_ + 1;
         }
         this.enableControl(_loc5_.TEAM_OPTIONS_ENABLE == "1" && _loc3_.length > 1,this.m_playerSlots[_loc4_].teamOptions);
         this.m_playerSlots[_loc4_].teamOptions.setData(_loc6_,_loc3_);
         this.m_playerSlots[_loc4_].teamOptions.setSelectedIndex(_loc7_);
         _loc4_ = _loc4_ + 1;
      }
   }
   function refreshFactionControls()
   {
      trace("GameSetupBase::refreshFactionControls()");
      var _loc2_ = 0;
      var _loc4_;
      var _loc3_;
      var _loc6_;
      var _loc5_;
      while(_loc2_ < this.m_playerSlots.length)
      {
         _loc4_ = new Object();
         loadVariables("QueryGameEngine?PLAYER_TEMPLATE_OPTIONS?Slot=" + String(_loc2_),_loc4_);
         _loc3_ = new Array();
         _loc3_ = _loc4_.PLAYER_TEMPLATE_OPTIONS_VALUES.split(",");
         _loc6_ = Number(_loc3_.shift());
         _loc5_ = new Array();
         _loc5_ = _loc4_.PLAYER_TEMPLATE_OPTIONS_IMAGES.split(",");
         this.enableControl(_loc3_.length > 1,this.m_playerSlots[_loc2_].factionOptions);
         this.m_playerSlots[_loc2_].factionOptions.setData(_loc5_,_loc3_);
         this.m_playerSlots[_loc2_].factionOptions.setSelectedIndex(_loc6_);
         _loc2_ = _loc2_ + 1;
      }
   }
   function refreshAIPersonalityControls()
   {
      trace("GameSetupBase::refreshAIPersonalityControls() : m_playerSlots.length = " + this.m_playerSlots.length);
      var _loc2_ = 0;
      var _loc5_;
      var _loc4_;
      var _loc7_;
      var _loc6_;
      var _loc3_;
      while(_loc2_ < this.m_playerSlots.length)
      {
         _loc5_ = new Object();
         loadVariables("QueryGameEngine?AI_PERSONALITY_OPTIONS?Slot=" + String(_loc2_),_loc5_);
         if(_loc5_.AI_PERSONALITY_OPTIONS == undefined)
         {
            this.m_playerSlots[_loc2_].personalityOptions.clear();
            this.m_playerSlots[_loc2_].personalityOptions.disable();
         }
         else
         {
            _loc4_ = new Array();
            _loc4_ = _loc5_.AI_PERSONALITY_OPTIONS.split(",");
            _loc7_ = Number(_loc4_.shift());
            _loc6_ = new Array();
            _loc3_ = 0;
            while(_loc3_ < _loc4_.length)
            {
               _loc6_[_loc3_] = "$APT:AI_PERSONALITY_" + String(_loc2_) + "_" + String(_loc4_[_loc3_]);
               _loc3_ = _loc3_ + 1;
            }
            this.enableControl(_loc4_.length > 1,this.m_playerSlots[_loc2_].personalityOptions);
            this.m_playerSlots[_loc2_].personalityOptions.setData(_loc6_,_loc4_);
            this.m_playerSlots[_loc2_].personalityOptions.setSelectedIndex(_loc7_);
         }
         _loc2_ = _loc2_ + 1;
      }
   }
   function refreshColorControls()
   {
      trace("GameSetupBase::refreshColorControls()");
      var _loc2_ = 0;
      var _loc3_;
      var _loc5_;
      var _loc6_;
      var _loc4_;
      while(_loc2_ < this.m_playerSlots.length)
      {
         _loc3_ = new Object();
         loadVariables("QueryGameEngine?PLAYER_COLOR?Slot=" + _loc2_,_loc3_);
         _loc5_ = new Array();
         _loc5_ = _loc3_.PLAYER_COLOR_VALUES.split(",");
         _loc6_ = Number(_loc5_.shift());
         _loc4_ = this.m_playerSlots[_loc2_].colorOptions;
         _loc4_.setData(_loc5_,_loc5_);
         _loc4_.setSelectedIndex(_loc6_);
         if(_loc3_.PLAYER_COLOR_ENABLE == "1")
         {
            _loc4_.enable();
         }
         else
         {
            _loc4_.disable();
         }
         _loc2_ = _loc2_ + 1;
      }
   }
   function refreshStartPositionControls()
   {
      trace("GameSetupBase::refreshStartPositionControls()");
      var _loc4_ = Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.mapPreview;
      var _loc2_ = 0;
      var _loc3_;
      var _loc7_;
      var _loc6_;
      var _loc5_;
      while(_loc2_ < this.m_playerStartPosMCArray.length)
      {
         _loc3_ = new Object();
         loadVariables("QueryGameEngine?START_POSITION?Position=" + String(_loc2_),_loc3_);
         _loc7_ = Boolean(_loc3_.START_POSITION_VALID);
         this.m_playerStartPosMCArray[_loc2_].setText("$START_POSITION_" + String(_loc2_));
         if(_loc7_)
         {
            _loc6_ = Number(_loc3_.START_POSITION_X);
            _loc5_ = Number(_loc3_.START_POSITION_Y);
            this.m_playerStartPosMCArray[_loc2_].show();
            this.m_playerStartPosMCArray[_loc2_].enable();
            this.m_playerStartPosMCArray[_loc2_]._x = _loc6_ * _loc4_._width + _loc4_._x;
            this.m_playerStartPosMCArray[_loc2_]._y = _loc5_ * _loc4_._height + _loc4_._y;
         }
         else
         {
            this.m_playerStartPosMCArray[_loc2_].disable();
            this.m_playerStartPosMCArray[_loc2_].hide();
         }
         _loc2_ = _loc2_ + 1;
      }
   }
   function refreshCampaignMapControls()
   {
      trace("GameSetupBase::refreshCampaignMapControls()");
      var _loc2_ = new Object();
      loadVariables("QueryGameEngine?PLAY_AS_CAMPAIGN",_loc2_);
      if(_loc2_.PLAY_AS_CAMPAIGN_VALUE == "0")
      {
         Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.campaignCheckbox.unCheck();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.campaignCheckbox.check();
      }
      if(_loc2_.PLAY_AS_CAMPAIGN_ENABLE == "0" || !this.m_isHost)
      {
         Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.campaignCheckbox.disable();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.gameSettings.mapPanel.campaignCheckbox.enable();
      }
   }
   function refreshResourcesControl()
   {
      trace("GameSetupBase::refreshResourcesControl()");
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?RESOURCES_OPTIONS",_loc1_);
      var _loc2_ = new Array();
      _loc2_ = _loc1_.RESOURCES_OPTIONS_VALUES.split(",");
      var _loc5_ = Number(_loc2_.shift());
      var _loc3_ = new Array();
      _loc3_ = _loc1_.RESOURCES_OPTIONS_STRINGS.split(",");
      var _loc4_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel.resourcesDropdown;
      _loc4_.setData(_loc3_,_loc2_);
      _loc4_.setSelectedIndex(_loc5_);
   }
   function refreshRulesCheckbox(ruleStr, checkbox)
   {
      trace("GameSetupBase::refreshRulesCheckbox() " + ruleStr + ":" + checkbox);
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?" + ruleStr,_loc1_);
      trace("  " + _loc1_[ruleStr + "_VALUE"]);
      if(_loc1_[ruleStr + "_VALUE"] == "0")
      {
         checkbox.unCheck();
      }
      else
      {
         checkbox.check();
      }
      trace("  " + _loc1_[ruleStr + "_ENABLE"]);
      if(_loc1_[ruleStr + "_ENABLE"] == "0")
      {
         checkbox.disable();
      }
      else
      {
         checkbox.enable();
      }
   }
   function refreshGameSpeedControls()
   {
      trace("GameSetupBase::refreshGameSpeedControls()");
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel.gameSpeedSlider;
      var _loc2_ = new Object();
      loadVariables("QueryGameEngine?GAMESPEED_OPTIONS",_loc2_);
      _loc3_.setIndexRange(Number(_loc2_.GAMESPEED_OPTIONS_MIN),Number(_loc2_.GAMESPEED_OPTIONS_MAX),Number(_loc2_.GAMESPEED_OPTIONS_CURRENT));
      if(_loc2_.GAMESPEED_OPTIONS_ENABLE == "0" || !this.m_isHost)
      {
         _loc3_.disable();
      }
      else
      {
         _loc3_.enable();
      }
   }
   function refreshReadyControls()
   {
      var _loc2_ = 0;
      var _loc3_;
      while(_loc2_ < this.m_playerSlots.length)
      {
         _loc3_ = this.m_playerSlots[_loc2_].readyCheckBox;
         this.refreshReadyCheckbox(_loc3_,_loc2_);
         _loc2_ = _loc2_ + 1;
      }
   }
   function refreshReadyCheckbox(checkbox, slotId)
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?READY_STATUS?Slot=" + String(slotId),_loc1_);
      _loc1_.READY_STATUS_ENABLED != "1" ? checkbox.disable() : checkbox.enable();
      _loc1_.READY_STATUS_VALUE != "1" ? checkbox.SetUnCheck() : checkbox.SetCheck();
   }
   function enableControl(enable, control)
   {
      if(enable)
      {
         control.enable();
      }
      else
      {
         control.disable();
      }
   }
}
#endinitclip
