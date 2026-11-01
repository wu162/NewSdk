function SetEnabled(val)
{
   enabled = val != 0;
   UpdateButtonGraphic();
}
function SetSelected(val)
{
   selected = val != 0;
   UpdateButtonGraphic();
}
function SetSecondState(val)
{
   secondState = val != 0;
   UpdateButtonGraphic();
}
function SetVisibility(vis)
{
   _visible = vis != 0;
}
function UpdateButtonGraphic()
{
   if(!enabled)
   {
      buttonContainerClip.gotoAndPlay("_disabled");
      return undefined;
   }
   if(mouseOver)
   {
      buttonContainerClip.gotoAndPlay("_over");
      return undefined;
   }
   if(selected)
   {
      buttonContainerClip.gotoAndPlay("_on");
      return undefined;
   }
   if(secondState)
   {
      buttonContainerClip.gotoAndPlay("_pressed");
      return undefined;
   }
   buttonContainerClip.gotoAndPlay("_off");
}
function OnPress()
{
   down = true;
   UpdateButtonGraphic();
}
function OnRelease()
{
   down = false;
   UpdateButtonGraphic();
   fscommand(qualifyName(this,"OnClicked"),"");
}
function OnRollOver()
{
   if(!mouseOver)
   {
      mouseOver = true;
      UpdateButtonGraphic();
      fscommand(qualifyName(this,"OnRollOver"),"");
   }
}
function OnRollOut()
{
   if(mouseOver)
   {
      mouseOver = false;
      UpdateButtonGraphic();
      fscommand(qualifyName(this,"OnRollOut"),"");
   }
}
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function SetHighlighted(highlightArg)
{
   var _loc2_ = highlightArg != 0;
   if(_loc2_)
   {
      if(highlightClip == undefined)
      {
         highlightClip = _global.CreateHUDGadgetFlash(buttonContainerClip);
      }
   }
   else if(highlightClip != undefined)
   {
      highlightClip.Close();
      highlightClip = undefined;
   }
}
function onUnload()
{
   highlightClip.Close();
   delete SetEnabled;
   delete SetSelected;
   delete SetSecondState;
   delete SetVisibility;
   delete UpdateButtonGraphic;
   delete OnPress;
   delete OnRelease;
   delete OnRollOver;
   delete OnRollOut;
   delete GetHelpSite;
   delete SetHighlighted;
   delete onUnload;
}
if(!initialized)
{
   enabled = true;
   down = false;
   mouseOver = false;
   selected = false;
   secondState = false;
   helpSite = eval(helpSiteRef);
   this.attachMovie(this.buttonContainer,"buttonContainerClip",this.getNextHighestDepth());
   UpdateButtonGraphic();
   hitTestButton._width = hitTestWidth;
   hitTestButton._height = hitTestHeight;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
