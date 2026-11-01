function AttachContent(id, contentType)
{
   id = Number(id);
   contentClip = attachMovie(contentType,String(id),id);
   return Pointer.encode(contentClip);
}
function DetachContent(id)
{
   id = Number(id);
   var _loc2_ = this[String(id)];
   if(_loc2_ != undefined)
   {
      _loc2_.removeMovieClip();
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
