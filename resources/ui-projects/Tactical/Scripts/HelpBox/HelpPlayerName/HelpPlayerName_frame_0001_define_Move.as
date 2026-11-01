function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function SetColor(colorArg)
{
   titleText.field.textColor = Number(colorArg);
}
function onUnload()
{
   delete Move;
   delete SetColor;
   delete onUnload;
}
if(!initialized)
{
   titleText.text = "$" + qualifyName(this,"Text") + "&outline";
   titleText.field.autoSize = "left";
   titleText.field._width;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
