function AttachContent(contentType)
{
   contentClip = attachMovie(contentType,"content",0);
   contentClip._x = placeholder._x;
   contentClip._y = placeholder._y;
   contentClip._width = placeholder._width;
   contentClip._height = placeholder._height;
   return Pointer.encode(contentClip);
}
function DetachContent()
{
   if(contentClip != undefined)
   {
      contentClip.removeMovieClip();
   }
}
function onUnload()
{
   delete AttachContent;
   delete DetachContent;
   delete onUnload;
}
if(!initialized)
{
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
