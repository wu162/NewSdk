onClipEvent(construct){
   m_refFM = "_global.gFM";
   m_bEnabled = false;
   m_initiallySelected = false;
   m_tabIndex = 0;
   m_bVisible = true;
   m_menuButtonSymbol = "menuButtonContentClipSymbol";
   m_menuItemSymbol = "menuItemContentClipSymbol";
   m_menuDefinitionArray = [];
   m_menuDefinitionArray[0] = "$Tutorial_MenuButton";
   m_menuDefinitionArray[1] = "$Campaign_MenuButton,$CampaignMenu_NewButton,$CampaignMenu_LoadButton,$CampaignMenu_ContinueButton,$CampaignMenu_ChaptersButton";
   m_menuDefinitionArray[2] = "$Versus_MenuButton,$Skirmish_MenuButton,$Online_MenuButton,$Network_MenuButton";
   m_menuDefinitionArray[3] = "$Profiles_MenuButton,$Manage_MenuButton,$Stats_MenuButton,$TransmissionLog_MenuButton,$ReplayTheater_MenuButton";
   m_menuDefinitionArray[4] = "$Options_MenuButton,$Setting_MenuButton,$Credits_MenuButton";
   m_menuDefinitionArray[5] = "$Exit_MenuButton";
}
