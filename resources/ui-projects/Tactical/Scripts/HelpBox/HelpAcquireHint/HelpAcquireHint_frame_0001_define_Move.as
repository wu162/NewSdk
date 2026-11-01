function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function SetAcquired(acquiredArg)
{
   var _loc1_ = Number(acquiredArg) != 0;
   text.field.textColor = !_loc1_ ? notAcquiredTextColor : acquiredTextColor;
}
function onUnload()
{
   delete Move;
   delete SetAcquired;
   delete onUnload;
}
if(!initialized)
{
   notAcquiredTextColor = 16776960;
   acquiredTextColor = 65280;
   text.text = "$" + qualifyName(this,"Text") + "&outline";
   text.field.textColor = notAcquiredTextColor;
   text.field.autoSize = "left";
   text.field._width;
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
stop();
