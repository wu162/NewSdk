function BindFunction(fn, obj)
{
   return function(buttonClip)
   {
      fn.call(obj,buttonClip);
   };
}
function Close()
{
   if(!closing)
   {
      gotoAndStop("_close");
      play();
      closing = true;
   }
}
function GetPurchaseButton(row, col)
{
   return Pointer.encode(dialog.rows[String(row)][String(col)]);
}
function SetProgressToNextRank(progress)
{
   var _loc1_ = Math.round(progress * 25) + 1;
   dialog.ProgressMeter.gotoAndStop(_loc1_);
}
function SetNumAvailablePoints(points)
{
   dialog.techPoints.gotoAndPlay("_" + String(points));
}
function OnConfirm()
{
   fscommand(qualifyName(this,"OnConfirmClicked"),"");
   if(!extern.InGame)
   {
      Close();
   }
}
function OnCancel()
{
   fscommand(qualifyName(this,"OnCancelClicked"),"");
   if(!extern.InGame)
   {
      Close();
   }
}
function OnClosed()
{
   fscommand(qualifyName(this,"OnClosed"),"");
}
function GetHelpBox()
{
   return Pointer.encode(helpBox);
}
function onUnload()
{
   delete BindFunction;
   delete Close;
   delete GetPurchaseButton;
   delete SetProgressToNextRank;
   delete SetNumAvailablePoints;
   delete OnConfirm;
   delete OnCancel;
   delete OnClosed;
   delete GetHelpBox;
   delete dialog.confirmButton.OnClicked;
   delete dialog.cancelButton.OnClicked;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   helpBox.loadMovie("HelpBox.swf");
   dialog.confirmButton.OnClicked = BindFunction(OnConfirm,this);
   dialog.cancelButton.OnClicked = BindFunction(OnCancel,this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
dialog._visible = false;
if(!extern.InGame)
{
   _global.qualifyName = function(clip, label)
   {
      return String(clip) + ":" + label;
   };
}
