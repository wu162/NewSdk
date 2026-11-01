function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   _visible = _loc1_;
}
function Open()
{
   panel.gotoAndPlay("_open");
}
function Close()
{
   panel.gotoAndPlay("_close");
}
function GetContentStage()
{
   return Pointer.encode(panel.stage);
}
function onUnload()
{
   delete Open;
   delete Close;
   delete GetContentStage;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   panel.stage._visible = false;
   panel.stop();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      var contentClip = panel.stage.attachMovie("TextContent","content",0);
      contentClip._width = panel.stage.placeholder._width;
      contentClip._height = panel.stage.placeholder._height;
      Open();
   }
}
stop();
