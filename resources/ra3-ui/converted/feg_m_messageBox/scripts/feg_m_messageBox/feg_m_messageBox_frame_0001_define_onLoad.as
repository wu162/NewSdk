function onLoad()
{
   trace("feg_m_messageBox::onLoad()");
   this.onEnterFrame = _global.bind0(this,frameDelayedInit);
}
function frameDelayedInit()
{
   this.onEnterFrame = null;
   m_refGlobalEventHandler = _root.gEH;
   this.messageBoxMC.button1.setOnMouseUpFunction(this.button1Clicked);
   this.messageBoxMC.button2.setOnMouseUpFunction(this.button2Clicked);
   this.messageBoxMC.button3.setOnMouseUpFunction(this.button3Clicked);
   this.messageBoxMC.button4.setOnMouseUpFunction(this.button4Clicked);
   gMH.addMessageHandler(localMessageHandler);
   this.messageBoxMC.button1.enable();
   this.messageBoxMC.button2.enable();
   this.messageBoxMC.button3.enable();
   this.messageBoxMC.button4.enable();
   this.messageBoxMC.button1.noIntro();
   this.messageBoxMC.button2.noIntro();
   this.messageBoxMC.button3.noIntro();
   this.messageBoxMC.button4.noIntro();
   this.messageBoxMC.button1.disable();
   this.messageBoxMC.button2.disable();
   this.messageBoxMC.button3.disable();
   this.messageBoxMC.button4.disable();
   onHide();
}
function button1Clicked()
{
   onHide();
   sendResponseToEngine(0);
}
function button2Clicked()
{
   onHide();
   sendResponseToEngine(1);
}
function button3Clicked()
{
   onHide();
   sendResponseToEngine(2);
}
function button4Clicked()
{
   onHide();
   sendResponseToEngine(3);
}
function onUnload()
{
   trace("feg_m_messageBox::onUnload()");
   gMH.removeMessageHandler(localMessageHandler);
   delete _global.feg_m_messageBox;
}
function localMessageHandler(messageCode)
{
   trace("feg_m_messageBox::localMessageHandler() messageCode = \'" + messageCode + "\'");
   var _loc2_ = false;
   switch(messageCode)
   {
      case _global.MSGCODE.FE_MESSAGE_SHOW:
         loadData();
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_MESSAGE_HIDE:
         onHide();
         _loc2_ = true;
   }
   return _loc2_;
}
function loadData()
{
   trace("feg_m_messageBox::loadData()");
   var _loc1_ = new Object();
   loadVariables("QueryGameEngine?MESSAGE_INFO",_loc1_);
   var _loc2_ = new Array();
   if(_loc1_.MESSAGE_INFO_OPTION1 != undefined && _loc1_.MESSAGE_INFO_OPTION1.length > 0)
   {
      _loc2_.push("$" + _loc1_.MESSAGE_INFO_OPTION1);
   }
   if(_loc1_.MESSAGE_INFO_OPTION2 != undefined && _loc1_.MESSAGE_INFO_OPTION2.length > 0)
   {
      _loc2_.push("$" + _loc1_.MESSAGE_INFO_OPTION2);
   }
   if(_loc1_.MESSAGE_INFO_OPTION3 != undefined && _loc1_.MESSAGE_INFO_OPTION3.length > 0)
   {
      _loc2_.push("$" + _loc1_.MESSAGE_INFO_OPTION3);
   }
   if(_loc1_.MESSAGE_INFO_OPTION4 != undefined && _loc1_.MESSAGE_INFO_OPTION4.length > 0)
   {
      _loc2_.push("$" + _loc1_.MESSAGE_INFO_OPTION4);
   }
   showMessageBox(_loc1_.MESSAGE_INFO_ID,"$" + _loc1_.MESSAGE_INFO_TITLE,"$" + _loc1_.MESSAGE_INFO_TEXT,_loc2_,sendResponseToEngine);
}
function showMessageBox(messageID, messageTitle, messageText, messageOptions, messageCallback)
{
   trace("feg_m_messageBox::showMessageBox()");
   m_messageID = messageID;
   m_messageCallback = messageCallback;
   if(m_refGlobalEventHandler.getCurrentLocalEventHandler() == undefined)
   {
      m_refGlobalEventHandler.setLocalEventHandler(null);
   }
   m_eventHandlerIndex = m_refGlobalEventHandler.addPriorityLocalEventHandler(localEventHandler,m_eventHandlerPriority);
   this.messageBoxMC.titleTF.text = messageTitle + "&Outline";
   this.messageBoxMC.messageTF.text = messageText + "&Outline";
   var messageBottom = this.messageBoxMC.messageTF._y + this.messageBoxMC.messageTF.textHeight;
   var bodyHeight = PADDING + this.messageBoxMC.messageTF.textHeight;
   m_handleEvents = false;
   if(messageOptions.length > 0)
   {
      m_handleEvents = true;
      this.messageBoxMC.button1.show();
      var buttonWidth = 2 * PADDING + this.messageBoxMC.button1.getWidth();
      var buttonHeight = this.messageBoxMC.button1._height;
      var buttonBarWidth = messageOptions.length * buttonWidth;
      var buttonBarX = (this.messageBoxMC.panelMiddleMC._width - buttonBarWidth) / 2 + this.messageBoxMC.panelMiddleMC._x;
      var btnMC;
      var index = 0;
      while(index < messageOptions.length)
      {
         btnMC = eval("this.messageBoxMC.button" + (index + 1));
         btnMC._y = messageBottom + PADDING;
         btnMC._x = buttonBarX + index * buttonWidth;
         btnMC.setText(messageOptions[index]);
         btnMC.show();
         btnMC.enable();
         index++;
      }
      bodyHeight += buttonHeight;
   }
   trace("feg_m_messageBox::showMessageBox() bodyHeight == " + bodyHeight);
   this.messageBoxMC.panelMiddleMC._height = bodyHeight;
   this.messageBoxMC.panelBottomMC._y = this.messageBoxMC.panelMiddleMC._y + this.messageBoxMC.panelMiddleMC._height - OVERLAP_OFFSET;
   onShow();
}
function localEventHandler(keyCode, controller)
{
   m_FM.handleDefaultInputs(keyCode);
}
function sendResponseToEngine(index)
{
   var _loc1_ = "%Message_Response?MessageID=" + m_messageID + "|ResponseID=" + index;
   trace("feg_m_messageBox::sendResponseToEngine() commandStr == " + _loc1_);
   fscommand("CallGameFunction",_loc1_);
}
function onShow()
{
   trace("feg_m_messageBox::onShow()");
   this.shadeMC._visible = true;
   var _loc2_ = 360;
   this.messageBoxMC._y = _loc2_ - this.messageBoxMC._height / 2;
   this.messageBoxMC._visible = true;
   m_playAudio = true;
}
function onHide()
{
   trace("feg_m_messageBox::onHide()");
   m_playAudio = false;
   this.shadeMC._visible = false;
   this.messageBoxMC._visible = false;
   this.messageBoxMC.titleTF.text = "";
   this.messageBoxMC.messageTF.text = "";
   this.messageBoxMC.button1._y = this.messageBoxMC.titleTF._y;
   this.messageBoxMC.button1.hide();
   this.messageBoxMC.button2._y = this.messageBoxMC.titleTF._y;
   this.messageBoxMC.button2.hide();
   this.messageBoxMC.button3._y = this.messageBoxMC.titleTF._y;
   this.messageBoxMC.button3.hide();
   this.messageBoxMC.button4._y = this.messageBoxMC.titleTF._y;
   this.messageBoxMC.button4.hide();
   if(m_eventHandlerIndex != -1)
   {
      m_refGlobalEventHandler.removeLocalEventHandler(m_eventHandlerIndex);
   }
   m_eventHandlerIndex = -1;
   m_handleEvents = false;
}
function playSoundFX()
{
   if(m_playAudio)
   {
      _global.playSound("Gui_GlobalShell_LoadingLights");
   }
}
trace("feg_m_messageBox loading now");
var PADDING = 5;
var m_messageID;
var m_messageCallback;
var m_playAudio = false;
var m_handleEvents = false;
var m_eventHandlerIndex = -1;
var m_eventHandlerPriority = 1;
var m_refGlobalEventHandler;
var OVERLAP_OFFSET = 4;
onLoad();
trace("feg_m_messageBox loaded");
stop();
