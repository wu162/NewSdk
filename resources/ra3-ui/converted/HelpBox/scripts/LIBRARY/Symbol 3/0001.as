function Resize(widthArg, heightArg)
{
   _width = Number(widthArg);
   _height = Number(heightArg);
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
