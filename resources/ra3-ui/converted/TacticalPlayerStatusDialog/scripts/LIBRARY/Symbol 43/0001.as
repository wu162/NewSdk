function OnCloseButtonClicked()
{
   _parent.Close();
}
function onUnload()
{
   delete OnCloseButtonClicked;
   delete onUnload;
}
closeButton.PressCallback = OnCloseButtonClicked;
