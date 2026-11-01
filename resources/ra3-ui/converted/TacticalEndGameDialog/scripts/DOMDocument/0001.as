function SetEndGameType(endGameType)
{
   panel.gotoAndPlay(endGameType);
}
function SetFaction(faction)
{
   panel.factionLabel = faction;
   panel.imageMC.gotoAndPlay(faction);
}
function onUnload()
{
   delete SetEndGameType;
   delete SetFaction;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   var backgroundX = _root.displayLeft;
   var backgroundY = _root.displayTop;
   var ancestor = this;
   while(ancestor != _root)
   {
      backgroundX -= ancestor._x;
      backgroundY -= ancestor._y;
      ancestor = ancestor._parent;
   }
   panel.background._x = backgroundX;
   panel.background._y = backgroundY;
   panel.background._width = _root.displayWidth;
   panel.background._height = _root.displayHeight;
   loopCount = 7;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
