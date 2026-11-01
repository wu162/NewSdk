if(!initialized)
{
   content._visible = false;
   if(_parent.letterBoxVisible)
   {
      gotoAndStop("_fadeIn");
      play();
   }
   initialized = true;
}
content._visible = false;
