function GetAbilityButtonStage(index)
{
   return Pointer.encode(abilities[String(index)].stage);
}
function SetState(state)
{
   gotoAndPlay(state);
}
function SetUnitCategory(arg)
{
   unitCategory = arg;
   unitCategoryInd.gotoAndPlay(unitCategory);
}
function SetWeaponCategory(arg)
{
   weaponCategory = arg;
   weaponCategoryInd.gotoAndPlay(weaponCategory);
}
function SetOreRemainingVisible(arg)
{
   if(Number(arg) == 0)
   {
      oreRemaining._visible = false;
   }
   else
   {
      oreRemaining._visible = true;
   }
}
function SetRank(rankArg, factionArg)
{
   rank = Math.min(Math.max(Number(rankArg),1),4);
   rankFactionLabel = String(factionArg);
   rankInd.factionLabel = String(factionArg);
   rankInd.gotoAndPlay("_" + String(rank));
   rankInd.image.gotoAndPlay(rankInd.factionLabel);
}
function SetHealth(curHealthArg, maxHealthArg)
{
   var _loc1_ = Number(maxHealthArg);
   if(_loc1_ != 0)
   {
      healthPct = Number(curHealthArg) / Number(maxHealthArg) * 100;
   }
   else
   {
      healthPct = 0;
   }
   var _loc2_ = healthPct + 1;
   healthBar.gotoAndStop(_loc2_);
}
function GetUpgradeIconList()
{
   return Pointer.encode(upgrades);
}
function GetPortraitHelpSite()
{
   return Pointer.encode(portraitHelpSite);
}
function SetAmmo(newAmmoCapacityArg, newAmmoLoadArg)
{
   var _loc1_ = Number(newAmmoCapacityArg);
   var _loc2_ = Number(newAmmoLoadArg);
   if(_loc1_ != ammoCapacity || _loc2_ != ammoLoad)
   {
      ammoCapacity = _loc1_;
      ammoLoad = _loc2_;
      if(ammoDisplay != undefined && ammoDisplay.initialized)
      {
         ammoDisplay.SetCapacity(_loc1_);
         ammoDisplay.SetLoad(_loc2_);
      }
   }
}
function GetCommandButton(buttonName)
{
   var _loc1_ = basicCommands[buttonName];
   if(_loc1_ == undefined)
   {
      _loc1_ = advCommands.panel[buttonName];
   }
   return Pointer.encode(_loc1_);
}
function SetInfiltrated(arg)
{
   infiltrated = arg != 0;
   infiltratedOverlay._visible = infiltrated;
}
function OpenAdvancedCommands()
{
   advCommands.gotoAndPlay("_open");
}
function CloseAdvancedCommands()
{
   advCommands.gotoAndPlay("_close");
}
function GetOpenAdvancedCommandsButtonHelpSite()
{
   return Pointer.encode(portraitHelpSite);
}
function SetOpenAdvancedCommandsButtonEnabled(enableArg)
{
   var _loc1_ = enableArg != 0;
   var _loc2_;
   if(_loc1_ != openAdvancedCommandsButtonEnabled)
   {
      _loc2_ = new Color(advCommands.openAdvancedCommandsButton);
      _loc2_.setRGB(!_loc1_ ? 8355711 : 16777215);
      if(!_loc1_)
      {
         advCommands.openAdvancedCommandsButton.gotoAndPlay("_up");
      }
      openAdvancedCommandsButtonEnabled = _loc1_;
   }
}
function SetOpenAdvancedCommandsButtonHighlighted(highlightArg)
{
   var _loc2_ = highlightArg != 0;
   if(_loc2_)
   {
      if(openAdvancedCommandsHighlightClip == undefined)
      {
         openAdvancedCommandsHighlightClip = _global.CreateHUDGadgetFlash(advCommands.highlightDummy);
      }
   }
   else if(openAdvancedCommandsHighlightClip != undefined)
   {
      openAdvancedCommandsHighlightClip.Close();
      openAdvancedCommandsHighlightClip = undefined;
   }
}
function onUnload()
{
   openAdvancedCommandsHighlightClip.Close();
   delete GetAbilityButtonStage;
   delete SetState;
   delete SetUnitCategory;
   delete SetWeaponCategory;
   delete SetOreRemainingVisible;
   delete SetRank;
   delete SetHealth;
   delete GetUpgradeIconList;
   delete GetPortraitHelpSite;
   delete SetLargeBarVisibility;
   delete SetAmmo;
   delete GetCommandButton;
   delete SetInfiltrated;
   delete OpenAdvancedCommands;
   delete CloseAdvancedCommands;
   delete GetOpenAdvancedCommandsButtonHelpSite;
   delete SetOpenAdvancedCommandsButtonEnabled;
   delete SetOpenAdvancedCommandsButtonHighlighted;
   delete onUnload;
}
if(!initialized)
{
   unitCategory = "_hide";
   weaponCategory = "_hide";
   rank = 1;
   healthPct = 100;
   ammoCapacity = 0;
   ammoLoad = 0;
   infiltrated = false;
   oreRemaining._visible = false;
   openAdvancedCommandsButtonEnabled = true;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      gotoAndStop("_singleUnit");
      play();
      SetUnitCategory("_infantry");
      SetWeaponCategory("_gun");
      SetUnitHealth(50,70);
      SetAmmo(0,5);
      healthPct = random(100);
   }
}
