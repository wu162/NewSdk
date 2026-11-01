function SetVisibility(vis)
{
   _visible = vis != 0;
}
function Move(x, y)
{
   _X = x;
   _Y = y;
}
function CreateMessage()
{
   messageClip.removeMovieClip();
   this.attachMovie("MpBeaconMessage","messageClip",0);
   messageClip._x = messagePos._x;
   messageClip._y = messagePos._y;
}
function RemoveMessage()
{
   messageClip.removeMovieClip();
}
function OnClicked()
{
   fscommand(qualifyName(this,"OnClicked"),"");
}
function onUnload()
{
   delete SetVisibility;
   delete Move;
   delete Destroy;
   delete CreateMessage;
   delete RemoveMessage;
   delete OnClicked;
   delete onUnload;
}
if(!initialized)
{
   arrow.client = this;
   if(beaconColor != undefined)
   {
      var colorizer = new Color(arrow.inner);
      colorizer.setRGB(beaconColor);
   }
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
