function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function SetUnitAbilityCategory(category)
{
   gotoAndPlay(category);
}
function onUnload()
{
   delete Move;
   delete SetUnitAbilityCategory;
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
