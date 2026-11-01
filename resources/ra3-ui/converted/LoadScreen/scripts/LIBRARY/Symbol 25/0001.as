function SetPercent(to)
{
   if(to >= vCurrentPrecent)
   {
      vCurrentPrecent = to;
      if(to <= 0)
      {
         _visible = false;
         Text.text = "";
      }
      else
      {
         _visible = true;
         Text.text = String(vCurrentPrecent) + "%" + "&dropShadow";
      }
      Bar.SetPercent(to);
   }
}
var vCurrentPrecent = -1;
SetPercent(-1);
