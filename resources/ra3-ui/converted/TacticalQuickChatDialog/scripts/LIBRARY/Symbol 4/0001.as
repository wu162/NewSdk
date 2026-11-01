if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
