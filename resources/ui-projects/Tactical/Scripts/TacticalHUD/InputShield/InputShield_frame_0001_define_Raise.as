function Raise()
{
   _visible = true;
}
function Lower()
{
   _visible = false;
}
function onUnload()
{
   delete Raise;
   delete Lower;
   delete onUnload;
}
if(!initialized)
{
   _visible = false;
   intialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
