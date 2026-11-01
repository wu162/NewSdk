function Move(x, y)
{
   relX = x;
   relY = y;
   _X = _parent.sizer._width * relX;
   _Y = _parent.sizer._height * relY;
}
function SetDisposition(disposition)
{
   icon.gotoAndPlay(disposition);
}
function onUnload()
{
   delete Move;
   delete SetDisposition;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
