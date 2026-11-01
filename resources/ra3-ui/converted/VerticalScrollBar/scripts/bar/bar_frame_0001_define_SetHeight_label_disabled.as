function SetHeight(newHeight)
{
   frame._height = newHeight;
   background._height = newHeight;
   bttnHeight = newHeight;
   if(bttn != undefined)
   {
      bttn._height = bttnHeight;
   }
}
function HandlePress()
{
}
function HandleRelease()
{
   _parent.OnSliderBarClicked(this);
}
function HandleRollOver()
{
   gotoAndStop("_over");
   play();
}
function HandleRollOut()
{
   gotoAndStop("_up");
   play();
}
function onUnload()
{
   delete SetHeight;
   delete HandlePress;
   delete HandleRelease;
   delete HandleRollOver;
   delete HandleRollOut;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
}
ImageLoader.attachMovie(param.disabledImage,"Image",0);
