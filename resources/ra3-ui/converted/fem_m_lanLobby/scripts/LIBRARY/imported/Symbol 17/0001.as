#initclip
class fem_m_lanLobby extends Cafe2_BaseUIScreen
{
   var m_gameNames;
   var m_localMessageHandlerFunc;
   var m_playerListCallbackFunc;
   var m_serverListCallbackFunc;
   function fem_m_lanLobby(screen)
   {
      super(screen);
      Cafe2_BaseUIScreen.m_thisClass = this;
      this.m_localMessageHandlerFunc = _global.bind1DynamicParams(this,this.localMessageHandler);
      this.m_serverListCallbackFunc = _global.bind2DynamicParams(this,this.onServerListboxMouseClick);
      this.m_playerListCallbackFunc = _global.bind2DynamicParams(this,this.onPlayerListboxMouseClick);
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
      fscommand("CallGameFunction","%LanInit");
      _global.GAME.MODE = _global.ENUM.GAME_MODE_LAN_MP;
   }
   function frameDelayedInit()
   {
      _global.gMH.addMessageHandler(this.m_localMessageHandlerFunc);
      this.initGUI();
   }
   function initGUI()
   {
      var _loc3_ = new Array();
      this.configureButton(Cafe2_BaseUIScreen.m_screen.joinButton,_global.bind0(this,this.joinButtonClicked));
      this.configureButton(Cafe2_BaseUIScreen.m_screen.createButton,_global.bind0(this,this.createButtonClicked));
      this.configureButton(Cafe2_BaseUIScreen.m_screen.backButton,_global.bind0(this,this.backButtonClicked));
      this.initServerList();
      this.initPlayerList();
      this.updateButtonState();
   }
   function configureButton(button, callback)
   {
      button.show();
      button.enable();
      button.setOnMouseUpFunction(callback);
      button.noIntro();
   }
   function onScreenExit()
   {
      _global.gMH.removeMessageHandler(this.m_localMessageHandlerFunc);
      delete this.m_localMessageHandlerFunc;
      Cafe2_BaseUIScreen.m_screen.serverList.removeListenerFunction(this.m_serverListCallbackFunc);
      Cafe2_BaseUIScreen.m_screen.serverList.shutdown();
      delete this.m_serverListCallbackFunc;
      Cafe2_BaseUIScreen.m_screen.playerList.removeListenerFunction(this.m_playerListCallbackFunc);
      Cafe2_BaseUIScreen.m_screen.playerList.shutdown();
      delete this.m_playerListCallbackFunc;
      this.clearData();
      delete _global.fem_m_lanLobby;
      delete _global.Cafe2_BaseUIScreen;
   }
   function localMessageHandler(message)
   {
      var _loc3_ = true;
      switch(message)
      {
         case _global.MSGCODE.FE_LAN_REFRESH_GAMELIST:
            this.populateGameList();
            break;
         case _global.MSGCODE.FE_LAN_REFRESH_PLAYERLIST:
            this.populatePlayerList();
            break;
         case _global.MSGCODE.FE_LAN_HOST_GAME_CREATED:
            this.startOuttro(_global.SCREEN.FEM_LAN_SETUP);
            break;
         case _global.MSGCODE.FE_LAN_CLIENT_GAME_JOINED:
            this.startOuttro(_global.SCREEN.FEM_LAN_SETUP);
            break;
         case _global.MSGCODE.FE_MP_LAN_LOBBY_STATE_CHANGE:
            this.updateButtonState();
            break;
         default:
            _loc3_ = false;
      }
      return _loc3_;
   }
   function populateGameList()
   {
      var _loc3_ = new Array();
      var _loc2_ = new Object();
      loadVariables("QueryGameEngine?LAN_GAMELIST",_loc2_);
      this.m_gameNames = _loc2_.LAN_GAMELIST_NAMES.split(",");
      Cafe2_BaseUIScreen.m_screen.serverList.setColumnData(0,this.m_gameNames);
      Cafe2_BaseUIScreen.m_screen.serverList.refreshDisplay();
      Cafe2_BaseUIScreen.m_screen.serverList.refreshScrollbar();
   }
   function populatePlayerList()
   {
      var _loc2_ = new Array();
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?LAN_PLAYERLIST",_loc1_);
      m_playerIds = _loc1_.LAN_PLAYERLIST_NAMES.split(",");
      Cafe2_BaseUIScreen.m_screen.playerList.setColumnData(0,m_playerIds);
      Cafe2_BaseUIScreen.m_screen.playerList.refreshDisplay();
      Cafe2_BaseUIScreen.m_screen.playerList.refreshScrollbar();
   }
   function backButtonClicked()
   {
      fscommand("CallGameFunction","%LanShutdown");
      _global.GAME.MODE = _global.ENUM.GAME_MODE_SHELL;
      super.backButtonClicked();
   }
   function createButtonClicked()
   {
      Cafe2_BaseUIScreen.m_screen.joinButton.disable();
      Cafe2_BaseUIScreen.m_screen.createButton.disable();
      fscommand("CallGameFunction","%LanCreateGame");
   }
   function joinButtonClicked()
   {
      Cafe2_BaseUIScreen.m_screen.joinButton.disable();
      Cafe2_BaseUIScreen.m_screen.createButton.disable();
      fscommand("CallGameFunction","%LanJoinGame?Game=" + Cafe2_BaseUIScreen.m_screen.serverList.getSelectedEntryIndex());
   }
   function initServerList()
   {
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.serverListScrollBar;
      _loc3_.show();
      _loc3_.enable();
      var _loc2_ = Cafe2_BaseUIScreen.m_screen.serverList;
      _loc2_.setNumEntries(15);
      _loc2_.setVerticalPadding(1);
      _loc2_.addListenerFunction(this.m_serverListCallbackFunc);
      var _loc4_ = new Array();
      _loc4_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:458,leftMargin:0,rightMargin:0});
      _loc2_.init(_loc4_,25);
      _loc2_.attachScrollbarController(_loc3_);
      _loc2_.moveViewToFirst();
      _loc2_.refreshDisplay();
      _loc2_.refreshScrollbar();
   }
   function initPlayerList()
   {
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.playerListScrollBar;
      _loc3_.show();
      _loc3_.enable();
      var _loc2_ = Cafe2_BaseUIScreen.m_screen.playerList;
      _loc2_.setNumEntries(20);
      _loc2_.setVerticalPadding(1);
      _loc2_.addListenerFunction(this.m_playerListCallbackFunc);
      var _loc4_ = new Array();
      _loc4_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:458,leftMargin:0,rightMargin:0});
      _loc2_.init(_loc4_,25);
      _loc2_.attachScrollbarController(_loc3_);
      _loc2_.moveViewToFirst();
      _loc2_.refreshDisplay();
      _loc2_.refreshScrollbar();
   }
   function onServerListboxMouseClick(index, prevIndex)
   {
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?LASTMOUSEPRESSTYPE&LASTMOUSEDOUBLECLICK",_loc3_);
      if(Number(_loc3_.LASTMOUSEPRESSTYPE) == _global.ENUM.MOUSEBUTTONPRESS_LEFT && Number(_loc3_.LASTMOUSEDOUBLECLICK) != 0)
      {
         this.joinButtonClicked();
      }
      else
      {
         this.updateButtonState();
      }
   }
   function onPlayerListboxMouseClick(index, prevIndex)
   {
   }
   function updateButtonState()
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?LAN_CAN_HOST_OR_JOIN",_loc1_);
      if(_loc1_.LAN_CAN_HOST_OR_JOIN == "1")
      {
         if(Cafe2_BaseUIScreen.m_screen.serverList.getSelectedEntryIndex() >= 0)
         {
            Cafe2_BaseUIScreen.m_screen.joinButton.enable();
         }
         else
         {
            Cafe2_BaseUIScreen.m_screen.joinButton.disable();
         }
         Cafe2_BaseUIScreen.m_screen.createButton.enable();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.joinButton.disable();
         Cafe2_BaseUIScreen.m_screen.createButton.disable();
      }
   }
}
#endinitclip
