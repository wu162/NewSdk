function OnDestroying()
{
   trace("SimpleRenderingSurface OnDestroying()");
   if(initialized)
   {
      fscommand(qualifyName(this,"OnClipDestroyed"),"");
   }
}
function onUnload()
{
   delete OnDestroying;
   delete onUnload;
}
if(!initialized)
{
   var initContent = {_type:qualifyName(this,"Render"),_x:shape._x,_y:shape._y,_width:shape._width,_height:shape._height};
   var contentClip = attachMovie("SimpleRenderingSurfaceContentPlaceholder","content",0,initContent);
   initialized = true;
}
