onClipEvent(construct){
   m_refFM = "_global.gFM";
   m_bEnabled = false;
   m_initiallySelected = false;
   m_tabIndex = 0;
   m_bVisible = true;
   m_menuButtonSymbol = "menuButtonContentClipSymbol";
   m_menuItemSymbol = "menuItemContentClipSymbol";
   m_menuDefinitionArray = [];
   m_menuDefinitionArray[0] = "$CampaignMenu_NewButton";
   m_menuDefinitionArray[1] = "$CampaignMenu_LoadButton";
   m_menuDefinitionArray[2] = "$CampaignMenu_ContinueButton";
   m_menuDefinitionArray[3] = "$CampaignMenu_ChaptersButton";
   m_menuDefinitionArray[4] = "$CampaignMenu_BackButton";
}
