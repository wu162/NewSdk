function Close()
{
   if(!closing)
   {
      closing = true;
      popup.gotoAndPlay("_close");
   }
}
function onUnload()
{
   delete Close;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   closing = false;
   if(_root.displayLeft != undefined)
   {
      var backdropX = _root.displayLeft;
      var backdropY = _root.displayTop;
      var backdropWidth = _root.displayWidth;
      var backdropHeight = _root.displayHeight;
      var clipPath = new Array();
      clip = this;
      while(clip != _root)
      {
         clipPath.push(clip);
         clip = clip._parent;
      }
      while(clipPath.length > 0)
      {
         var clip = clipPath.pop();
         backdropX -= clip._x;
         backdropY -= clip._y;
         var invScaleX = 100 / clip._xscale;
         var invScaleY = 100 / clip._yscale;
         backdropX *= invScaleX;
         backdropY *= invScaleY;
         backdropWidth *= invScaleX;
         backdropHeight *= invScaleY;
      }
      backdrop._x = backdropX;
      backdrop._y = backdropY;
      backdrop._width = backdropWidth;
      backdrop._height = backdropHeight;
   }
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
