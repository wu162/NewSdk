function Show()
{
   gotoAndStop("_show");
   play();
}
function Hide()
{
   gotoAndStop("_hide");
   play();
}
onUnload = function()
{
   delete Show;
   delete Hide;
   delete onUnload;
};
if(_parent.isSelected)
{
   gotoAndStop("_show");
   play();
}
else
{
   stop();
}
