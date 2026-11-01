function Open()
{
   this.coopControl.gotoAndPlay("_opening");
}
function Close()
{
   this.coopControl.gotoAndPlay("_closing");
}
function SetVisibility(visArg)
{
   _visible = Number(visArg) != 0;
}
function GetMonitor()
{
   return Pointer.encode(this.coopControl.monitor);
}
function GetNormalButton()
{
   return Pointer.encode(this.coopControl.normalButton);
}
function GetAssaultButton()
{
   return Pointer.encode(this.coopControl.assaultButton);
}
function GetHoldObjectButton()
{
   return Pointer.encode(this.coopControl.holdObjectButton);
}
function GetHoldPositionButton()
{
   return Pointer.encode(this.coopControl.holdPositionButton);
}
function InitializeButtons()
{
}
function setHouseColor(houseColor)
{
   var _loc1_ = new Color(coopControl.portrait.houseColorMC);
   _loc1_.setRGB(houseColor);
}
function SetHighlighted(highlightArg)
{
   var _loc2_ = highlightArg != 0;
   if(_loc2_)
   {
      if(highlightClip == undefined)
      {
         highlightClip = _global.CreateHUDGadgetFlash(coopControl.highlightDummy);
      }
   }
   else if(highlightClip != undefined)
   {
      highlightClip.Close();
      highlightClip = undefined;
   }
}
function SetPortraitHighlighted(highlightArg)
{
   var _loc1_ = highlightArg != 0;
   if(_loc1_)
   {
      if(!coopControl.portraitFlashMC.isOn)
      {
         coopControl.portraitFlashMC.gotoAndPlay("_on");
         coopControl.portraitFlashMC.isOn = true;
      }
      coopControl.portraitFlashMC.done = false;
   }
   else if(coopControl.portraitFlashMC.isOn)
   {
      coopControl.portraitFlashMC.done = true;
   }
}
function onUnload()
{
   highlightClip.Close();
   delete Open;
   delete Close;
   delete SetVisibility;
   delete GetMonitor;
   delete GetDefaultButton;
   delete GetAssaultButton;
   delete GetHoldObjectButton;
   delete GetHoldPositionButton;
   delete InitializeButtons;
   delete SetHighlighted;
   delete SetPortraitHighlighted;
   delete onUnload;
}
if(!initialized)
{
   trace("Co Op AI Order Panenl Initialized");
   SetVisibility(true);
   InitializeButtons();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      Open();
      SetPortraitHighlighted(1);
   }
}
itemStage._visible = false;
