function SelectColorButton(colorIndex)
{
   var _loc1_ = colors[String(colorIndex)];
   selectedColorButton.gotoAndPlay("_up");
   selectedColorButton = _loc1_;
   _loc1_.gotoAndPlay("_selected");
}
function OnColorButtonClicked(colorButton)
{
   _parent._parent.OnColorClicked(Number(colorButton._name));
}
function SelectLineWidthButton(lineWidthIndex)
{
   var _loc1_ = lineWidths[String(lineWidthIndex)];
   selectedLineWidthButton.gotoAndPlay("_up");
   selectedLineWidthButton = _loc1_;
   _loc1_.gotoAndPlay("_selected");
}
function OnLineWidthButtonClicked(lineWidthButton)
{
   _parent._parent.OnLineWidthClicked(Number(lineWidthButton._name));
}
function onUnload()
{
   delete SelectColorButton;
   delete OnColorButtonClicked;
   delete SelectLineWidthButton;
   delete OnLineWidthButtonClicked;
   delete onUnload;
}
if(!initialized)
{
   var selectedColorIndex = _parent._parent.selectedColorIndex;
   if(selectedColorIndex != undefined)
   {
      selectedColorButton = colors[String(selectedColorIndex)];
      selectedColorButton.gotoAndPlay("_selected");
   }
   var selectedLineWidthIndex = _parent._parent.selectedLineWidthIndex;
   if(selectedLineWidthIndex != undefined)
   {
      selectedLineWidthButton = lineWidths[String(selectedLineWidthIndex)];
      selectedLineWidthButton.gotoAndPlay("_selected");
   }
   initialized = true;
}
