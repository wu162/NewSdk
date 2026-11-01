// WARNING: This class was decompiled without detecting uninitialized class fields.
class scrollColumnDefinition
{
   function scrollColumnDefinition()
   {
   }
   function init()
   {
      this.m_dataArray = new Array();
      this.m_iconStates = new Array();
      this.m_iconStateLabels = new Array();
      this.m_iconDefArray = new Array();
   }
   function setData(dataArray)
   {
      this.m_dataArray = new Array();
      var _loc2_ = 0;
      while(_loc2_ < dataArray.length)
      {
         this.m_dataArray[_loc2_] = dataArray[_loc2_];
         _loc2_ = _loc2_ + 1;
      }
   }
   function getData()
   {
      return this.m_dataArray;
   }
   function setIconStates(statesArray)
   {
      this.m_iconStates = new Array();
      var _loc2_ = 0;
      while(_loc2_ < statesArray.length)
      {
         this.m_iconStates[_loc2_] = statesArray[_loc2_];
         _loc2_ = _loc2_ + 1;
      }
   }
   function getIconStates()
   {
      return this.m_iconStates;
   }
   function setNumOfVisColumn(numOfVisColumns)
   {
      this.m_visibleLinesPerCol = numOfVisColumns;
   }
   function getNumOfVisColumn()
   {
      return this.m_visibleLinesPerCol;
   }
   function setBufferSize(bufferSize)
   {
      this.m_bufferSize = bufferSize;
   }
   function getBufferSize()
   {
      return this.m_bufferSize;
   }
   function setVerticalPadding(vertPadding)
   {
      this.m_vertPadding = vertPadding;
   }
   function getVerticalPadding()
   {
      return this.m_vertPadding;
   }
   function setIconDefArray(iconDefArray)
   {
      this.m_iconDefArray = new Array();
      var _loc2_ = 0;
      while(_loc2_ < iconDefArray.length)
      {
         this.m_iconDefArray[_loc2_] = iconDefArray[_loc2_];
         _loc2_ = _loc2_ + 1;
      }
   }
   function getIconDefArray()
   {
      return this.m_iconDefArray;
   }
   function setWidth(theWidth)
   {
      this.m_width = theWidth;
   }
   function getWidth()
   {
      return this.m_width;
   }
}
