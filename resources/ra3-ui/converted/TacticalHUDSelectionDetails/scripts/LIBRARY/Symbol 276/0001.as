function OnContentLoaded(clip)
{
   fscommand(qualifyName(this,"OnContentLoaded"),Pointer.encode(clip));
}
function LoadContent(url)
{
   this.createEmptyMovieClip("content",0);
   this.content.loadMovie(url);
}
function UnloadContent()
{
   content.removeMovieClip();
}
function onUnload()
{
   delete OnContentLoaded;
   delete LoadContent;
   delete UnloadContent;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
