function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function SetAmphibious(val)
{
   gotoAndPlay(val == 0 ? "NotAmphibious" : "Amphibious");
}
function onUnload()
{
   delete Move;
   delete SetAmphibious;
   delete onUnload;
}
if(!initialized)
{
   text.text = "$" + qualifyName(this,"Text") + "&outline";
   text.field.autoSize = "left";
   text.field._width;
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
