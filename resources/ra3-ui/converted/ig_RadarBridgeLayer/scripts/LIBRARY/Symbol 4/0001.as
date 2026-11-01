function Move(relX, relY)
{
   _parent.MoveIndicator(this,relX,relY);
}
function SetOrientation(orient)
{
   _rotation = orient * 180 / 3.1415927410125732;
}
function SetScale(widthScale, lengthScale)
{
   _parent.ScaleIndicator(this,widthScale,lengthScale);
}
function onUnload()
{
   delete Move;
   delete SetOrientation;
   delete SetScale;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(_global.qualifyName(this,"OnInitialized"),"");
}
