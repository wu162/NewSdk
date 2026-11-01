if(!initialized)
{
   gotoAndPlay(category);
   bonus._visible = isBonus;
   primary._visible = !isBonus;
   initialized = true;
}
