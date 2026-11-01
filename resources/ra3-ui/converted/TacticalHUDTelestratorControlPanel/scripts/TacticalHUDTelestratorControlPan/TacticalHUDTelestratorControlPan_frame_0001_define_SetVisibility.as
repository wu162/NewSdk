function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   panel._visible = _loc1_;
}
function Open()
{
   if(!isOpen)
   {
      panel.titleBox.openToggle.gotoAndStop("_maximize");
      panel.gotoAndPlay("_open");
      panel.titleBox.TitleFrame.gotoAndPlay("_max");
      isOpen = true;
   }
}
function Close()
{
   if(isOpen)
   {
      panel.titleBox.openToggle.gotoAndStop("_minimize");
      panel.gotoAndPlay("_close");
      panel.titleBox.TitleFrame.gotoAndPlay("_mini");
      isOpen = false;
   }
}
function OnOpenToggleClicked()
{
   if(extern.InGame)
   {
      fscommand(qualifyName(this,"OnOpenToggleClicked"),"");
   }
   else if(isOpen)
   {
      Close();
   }
   else
   {
      Open();
   }
}
function SelectColor(colorIndexArg)
{
   var _loc1_ = Number(colorIndexArg);
   if(_loc1_ != selectedColorIndex)
   {
      selectedColorIndex = _loc1_;
      panel.box.SelectColorButton(_loc1_);
   }
}
function OnColorClicked(colorIndex)
{
   if(extern.InGame)
   {
      fscommand(qualifyName(this,"OnColorClicked"),String(colorIndex));
   }
   else
   {
      SelectColor(colorIndex);
   }
}
function SelectLineWidth(lineWidthIndexArg)
{
   var _loc1_ = Number(lineWidthIndexArg);
   if(_loc1_ != selectedLineWidthIndex)
   {
      selectedLineWidthIndex = _loc1_;
      panel.box.SelectLineWidthButton(_loc1_);
   }
}
function OnLineWidthClicked(lineWidthIndex)
{
   if(extern.InGame)
   {
      fscommand(qualifyName(this,"OnLineWidthClicked"),String(lineWidthIndex));
   }
   else
   {
      SelectLineWidth(lineWidthIndex);
   }
}
function OnEraseAllClicked()
{
   fscommand(qualifyName(this,"OnEraseAllClicked"),"");
}
function onUnload()
{
   delete Open;
   delete Close;
   delete OnOpenToggleClicked;
   delete SelectColor;
   delete OnColorClicked;
   delete SelectLineWidth;
   delete OnLineWidthClicked;
   delete OnEraseAllClicked;
   delete SetVisibility;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   isOpen = false;
   selectedColorIndex = 0;
   selectedLineWidthIndex = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
