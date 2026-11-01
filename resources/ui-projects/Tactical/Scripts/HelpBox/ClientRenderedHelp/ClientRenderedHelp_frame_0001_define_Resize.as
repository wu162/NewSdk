function Resize(widthArg, heightArg)
{
   _width = Number(widthArg);
   _height = Number(heightArg);
}
if(!initialized)
{
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
stop();
