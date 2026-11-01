function OnContentLoaded(clip)
{
   if(clip == energyMeter)
   {
      energyMeter.helpSite = energyMeterHelpSite;
   }
}
function onUnload()
{
   delete OnContentLoaded;
   delete onUnload;
}
if(!initialized)
{
   commandModeBar.loadMovie("TacticalHUDCommandModeBar.swf");
   energyMeter.loadMovie("TacticalHUDEnergyMeter.swf");
   radar.loadMovie("TacticalHUDRadar.swf");
   mainCommandBar.loadMovie("TacticalHUDMainCommandBar.swf");
   initialized = true;
}
stop();
