function SetAffordable(valArg)
{
   var _loc1_ = valArg != 0;
   textClip.field.textColor = !_loc1_ ? 16711680 : 16777215;
   gotoAndStop(!_loc1_ ? "_cantAfford" : "_canAfford");
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
   delete SetAffordable;
   delete Move;
   delete onUnload;
}
if(!initialized)
{
   var rightMargin = background._x + background._width - (textClip._x + textClip._width);
   textClip.text = "$" + qualifyName(this,"Text") + "&outline";
   textClip.field.autoSize = "left";
   textClip.field._width;
   background._width = textClip._x + textClip._width + rightMargin - background._x;
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
stop();
