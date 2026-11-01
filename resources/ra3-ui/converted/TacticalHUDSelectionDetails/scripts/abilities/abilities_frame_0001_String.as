if(!initialized)
{
   var i = 0;
   while(i < 1)
   {
      this[String(i)].stage.helpSite = this["helpSite" + String(i)];
      i++;
   }
   initialized = true;
}
stop();
