function SetAffordable(valArg)
{
   var _loc1_ = valArg != 0;
   textClip.field.textColor = !_loc1_ ? 16711680 : 16777215;
}
function OnMasterResize(mc)
{
   var _loc1_ = mc.contentStage._x + _X;
   background._x = - _loc1_;
}
function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
   background._width = Number(widthArg) + 5;
}
function onUnload()
{
   _parent._parent.UnregisterMasterResizeListener(this);
   delete SetAffordable;
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
