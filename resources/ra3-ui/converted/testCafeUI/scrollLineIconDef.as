// WARNING: This class was decompiled without detecting uninitialized class fields.
class scrollLineIconDef
{
   function scrollLineIconDef()
   {
   }
   function init(iconStates, iconSymbolName)
   {
      this.m_numberOfStates = iconStates.length;
      this.m_iconStatesArray = iconStates;
      this.m_iconSymbolName = iconSymbolName;
   }
   function getIconStatus(iconNumber)
   {
      return this.m_iconStatus[iconNumber];
   }
   function getNumberOfStates()
   {
      return this.m_numberOfStates;
   }
   function getIconSymbolName()
   {
      return this.m_iconSymbolName;
   }
   function getState(stateID)
   {
      return this.m_iconStatesArray[stateID];
   }
}
