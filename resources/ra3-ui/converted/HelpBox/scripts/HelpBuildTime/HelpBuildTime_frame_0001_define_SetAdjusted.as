function SetAdjusted(valArg)
{
   var _loc1_ = valArg != 0;
   textClip.field.textColor = !_loc1_ ? 16777215 : 16711680;
}
function OnMasterResize(mc)
{
   background._width = mc.contentStage._width - _X - 6;
}
function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function onUnload()
{
   _parent._parent.UnregisterMasterResizeListener(this);
   delete SetAdjusted;
   delete Move;
   delete OnMasterResize;
   delete onUnload;
}
if(!initialized)
{
   _parent._parent.RegisterMasterResizeListener(this);
   var rightMargin = background._x + background._width - (textClip._x + textClip._width);
   textClip.text = "$" + qualifyName(this,"Text") + "&outline";
   textClip.field.autoSize = "left";
   textClip.field._width;
   background._width = textClip._x + textClip._width + rightMargin - background._x;
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
stop();
