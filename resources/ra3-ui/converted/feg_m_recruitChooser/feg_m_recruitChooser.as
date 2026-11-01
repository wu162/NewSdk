// WARNING: This class was decompiled without detecting uninitialized class fields.
class feg_m_recruitChooser extends Cafe2_BaseUIScreen
{
   static var HELP_BOX_WIDTH = 462;
   static var HELP_BOX_HEIGHT = 203;
   function feg_m_recruitChooser(screen)
   {
      super(screen);
      Cafe2_BaseUIScreen.m_screen.screenMC._visible = false;
      Cafe2_BaseUIScreen.m_thisClass = this;
      this.m_playerSelectedHandler = _global.bind1DynamicParams(this,this.onPlayerSelected);
      this.m_friendSelectedHandler = _global.bind1DynamicParams(this,this.onFriendSelected);
      this.m_localMessageHandlerFunc = _global.bind1DynamicParams(this,this.localMessageHandler);
      this.m_selectedPlayerId = "0";
   }
   function frameDelayedInit()
   {
      _global.gMH.addMessageHandler(this.m_localMessageHandlerFunc);
      _global.GAME.MODE = _global.ENUM.GAME_MODE_MULTIPLAYER;
      this.initGUI();
   }
   function initGUI()
   {
      var _loc3_ = new Array();
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsListMC.friendsMC.buddylist.addlistener(this.m_friendSelectedHandler);
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC.recruitmentPlayerListMC.addlistener(this.m_playerSelectedHandler);
      this.configureButton(Cafe2_BaseUIScreen.m_screen.backButton,_global.bind0(this,this.onCancelCoopRecruitment),_loc3_);
      this.configureButton(Cafe2_BaseUIScreen.m_screen.screenMC.inviteButton,_global.bind0(this,this.onInviteButtonClicked),_loc3_);
      _loc3_.push(Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.chatComponentMC.sendButton);
      _loc3_.push(Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC.recruitmentLobbyMC.GetListToggler());
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsTabButton.show();
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsTabButton.enable();
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsTabButton.setOnMouseDownFunction(_global.bind0(this,this.onFriendsTab));
      _loc3_.push(Cafe2_BaseUIScreen.m_screen.screenMC.friendsTabButton);
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbyTabButton.show();
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbyTabButton.enable();
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbyTabButton.setOnMouseDownFunction(_global.bind0(this,this.onLobbyTab));
      _loc3_.push(Cafe2_BaseUIScreen.m_screen.screenMC.lobbyTabButton);
      this.setHostProfileName();
      this.setInviteeProfileName();
      this.initHelpPanel(["$RECRUITCENTERHELPTEXTLOBBIES"],Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.lobbyHelpBox.helpTextListBox,Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.lobbyHelpBox.helpTextScrollBar);
      this.initHelpPanel(["$RECRUITCENTERHELPTEXTFRIENDS"],Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.friendsHelpBox.helpTextListBox,Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.friendsHelpBox.helpTextScrollBar);
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
      Cafe2_BaseUIScreen.m_screen.screenMC.tabButtonController.SetSelectedRadioButton(Cafe2_BaseUIScreen.m_screen.screenMC.lobbyTabButton);
      this.onLobbyTab();
      this.registerIntroOuttroComponents(_loc3_);
      this.startIntro();
      Cafe2_BaseUIScreen.m_screen.screenMC._visible = true;
   }
   function configureButton(button, callback, ioButtons)
   {
      button.show();
      button.enable();
      button.setOnMouseUpFunction(callback);
      ioButtons.push(button);
   }
   function localMessageHandler(messageCode)
   {
      var _loc3_ = true;
      switch(messageCode)
      {
         case _global.MSGCODE.FE_MP_ONLINE_REFRESH_GAMEPLAYERS:
            this.startOuttro(_global.SCREEN.FEM_COOPSTAGING);
            break;
         case _global.MSGCODE.FE_MP_ONLINE_INVITE_RECEIVED:
            _global.comLink.screen.open();
            _loc3_ = false;
            break;
         default:
            _loc3_ = false;
      }
      return _loc3_;
   }
   function onCancelCoopRecruitment()
   {
      fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
      _global.GAME.COOPMODE = _global.ENUM.GAME_COOPMODE_CHOOSER;
      this.backButtonClicked();
   }
   function onScreenExit()
   {
      _global.gMH.removeMessageHandler(this.m_localMessageHandlerFunc);
      delete this.m_localMessageHandlerFunc;
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsListMC.friendsMC.buddylist.removeListener(this.m_friendSelectedHandler);
      delete this.m_friendSelectedHandler;
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC.recruitmentPlayerListMC.removeListener(this.m_playerSelectedHandler);
      delete this.m_playerSelectedHandler;
      this.clearData();
      delete _global.feg_m_recruitChooser;
      delete _global.Cafe2_BaseUIScreen;
   }
   function onInviteButtonClicked()
   {
      if(Number(this.m_selectedPlayerId) != 0)
      {
         fscommand("CallGameFunction","%OnlineInvitePlayer?Player=" + this.m_selectedPlayerId);
      }
   }
   function setHostProfileName()
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?FE_CURRENT_PROFILE",_loc1_);
      if(_loc1_.FE_CURRENT_PROFILE == undefined)
      {
         _loc1_.FE_CURRENT_PROFILE = "THE HOST";
      }
      Cafe2_BaseUIScreen.m_screen.screenMC.commanderPanelMC.commanderNameTF.text = _loc1_.FE_CURRENT_PROFILE;
   }
   function setInviteeProfileName()
   {
      var _loc2_ = true;
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?FE_CO_COMMANDER_PROFILE",_loc1_);
      if(_loc1_.FE_CO_COMMANDER_PROFILE == undefined)
      {
         _loc1_.FE_CO_COMMANDER_PROFILE = "$OPEN";
         _loc2_ = false;
      }
      Cafe2_BaseUIScreen.m_screen.screenMC.commanderPanelMC.co_commanderNameTF.text = _loc1_.FE_CO_COMMANDER_PROFILE;
      return _loc2_;
   }
   function onFriendsTab()
   {
      this.showFriendsList();
      this.hideLobbyList();
      this.hideLobbyHelpText();
      this.showFriendsHelpText();
      this.clearInviteSelection();
   }
   function onLobbyTab()
   {
      this.hideFriendsList();
      this.showLobbyList();
      this.showLobbyHelpText();
      this.hideFriendsHelpText();
      this.clearInviteSelection();
   }
   function clearInviteSelection()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsListMC.friendsMC.buddylist.listbox.setSelectedIndex(std_mouseScrollingListbox.NO_SELECTION);
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC.recruitmentPlayerListMC.playerListTextBox.setSelectedIndex(std_mouseScrollingListbox.NO_SELECTION);
      this.m_selectedPlayerId = "0";
      this.updateInviteButton();
   }
   function hideFriendsList()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsListMC._visible = false;
   }
   function showFriendsList()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.friendsListMC._visible = true;
   }
   function hideLobbyList()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC._visible = false;
   }
   function showLobbyList()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC._visible = true;
   }
   function updateInviteButton()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.inviteButton.disable();
      var _loc2_;
      if(Number(this.m_selectedPlayerId) != 0)
      {
         _loc2_ = new Object();
         loadVariables("QueryGameEngine?MP_BUDDYSTATUS?Player=" + this.m_selectedPlayerId + "&IS_PLAYER_BUDDY?Player=" + this.m_selectedPlayerId,_loc2_);
         if(Number(_loc2_.IS_PLAYER_BUDDY) == 0 || Number(_loc2_.MP_BUDDYSTATUS_IS_ONLINE) != 0)
         {
            Cafe2_BaseUIScreen.m_screen.screenMC.inviteButton.enable();
         }
      }
   }
   function onFriendSelected(index)
   {
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?LASTMOUSEPRESSTYPE",_loc3_);
      this.m_selectedPlayerId = Cafe2_BaseUIScreen.m_screen.screenMC.friendsListMC.friendsMC.buddylist.getBuddyIdAt(index);
      this.updateInviteButton();
      if(Number(_loc3_.LASTMOUSEPRESSTYPE) == _global.ENUM.MOUSEBUTTONPRESS_RIGHT)
      {
         _global.contextMenu.screen.doDefaultFriendContextMenu(this.m_selectedPlayerId,_global.bind1DynamicParams(this,this.onContextMenuEntrySelected));
      }
   }
   function onPlayerSelected(index)
   {
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?LASTMOUSEPRESSTYPE",_loc3_);
      this.m_selectedPlayerId = Cafe2_BaseUIScreen.m_screen.screenMC.lobbiesMC.recruitmentPlayerListMC.getPlayerID();
      this.updateInviteButton();
      if(Number(_loc3_.LASTMOUSEPRESSTYPE) == _global.ENUM.MOUSEBUTTONPRESS_RIGHT)
      {
         _global.contextMenu.screen.doDefaultPlayerContextMenu(this.m_selectedPlayerId,_global.bind1DynamicParams(this,this.onContextMenuEntrySelected));
      }
   }
   function onContextMenuEntrySelected(id)
   {
      var _loc0_;
      var _loc3_;
      if((_loc0_ = id) === _global.ENUM.CONTEXTMENU_WHISPER)
      {
         _loc3_ = new Object();
         loadVariables("QueryGameEngine?WHISPER_PREFIX?Player=" + this.m_selectedPlayerId,_loc3_);
         Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.chatComponentMC.chatInputBox.setContents(String(_loc3_.WHISPER_PREFIX));
         Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.chatComponentMC.chatInputBox.takeFocus();
      }
   }
   function hideLobbyHelpText()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.lobbyHelpBox._visible = false;
   }
   function showLobbyHelpText()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.lobbyHelpBox._visible = true;
   }
   function hideFriendsHelpText()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.friendsHelpBox._visible = false;
   }
   function showFriendsHelpText()
   {
      Cafe2_BaseUIScreen.m_screen.screenMC.chatPanelMC.friendsHelpBox._visible = true;
   }
   function initHelpPanel(listData, listMC, listScrollBar)
   {
      listScrollBar.show();
      listScrollBar.enable();
      listMC.setRenderSize(HELP_BOX_HEIGHT,HELP_BOX_WIDTH);
      listMC.setVerticalPadding(0);
      listMC.disableEntryHitRegion();
      var _loc2_ = new Array();
      _loc2_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:HELP_BOX_WIDTH,leftMargin:0,rightMargin:0,useHighlights:false,isSelectable:false,useHitRegion:false,autoSize:true});
      listMC.init(_loc2_,29);
      listMC.attachScrollbarController(listScrollBar);
      listMC.setColumnData(0,listData);
      listMC.setSelectedIndex(0);
      listMC.refreshDisplay();
      listMC.refreshScrollbar();
      listMC.moveViewToFirst();
   }
}
