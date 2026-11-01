// WARNING: This class was decompiled without detecting uninitialized class fields.
class std_mouseScrollbar_logic extends MovieClip
{
   var m_DragThumbToolb = false;
   static var CONTROL_THUMB = 0;
   static var CONTROL_INCREMENT_ARROW = 1;
   static var CONTROL_DECREMENT_ARROW = 2;
   function std_mouseScrollbar_logic()
   {
      super();
      if(this.m_nMin == undefined || this.m_nMax == undefined || this.m_nValue == undefined)
      {
         trace("WARNING: invalid component properties - " + this);
      }
      this.m_index = new CafeFW_IndexMgr();
      this.m_index.SetInfo(this.m_nValue,this.m_nMin,this.m_nMax,1,false,false);
      this.m_scrollbarEventListeners = new Array();
   }
   static function defaultInitObject()
   {
      var _loc1_ = new Object();
      _loc1_.m_nMin = 0;
      _loc1_.m_nMax = 10;
      _loc1_.m_nValue = 0;
      return _loc1_;
   }
   function getValue()
   {
      return this.m_index.GetIndex();
   }
   function setValue(p_nValue)
   {
      if(this.m_index.SetIndex(p_nValue) != CafeFW_IndexMgr.TIBINDEX_CHANGE_NONE)
      {
         this.update();
      }
   }
   function nextValue()
   {
      if(this.m_index.IncIndex() != CafeFW_IndexMgr.TIBINDEX_CHANGE_NONE)
      {
         this.update();
         this.onNext();
      }
   }
   function prevValue()
   {
      if(this.m_index.DecIndex() != CafeFW_IndexMgr.TIBINDEX_CHANGE_NONE)
      {
         this.update();
         this.onPrev();
      }
   }
   function getMin()
   {
      return this.m_index.GetMinIndex();
   }
   function getMax()
   {
      return this.m_index.GetMaxIndex();
   }
   function setRange(range)
   {
      trace("WARNING: Cafe2_Imp_StdScrollbar::setRange() deprecated use setScrollbarInfo");
      this.setInfo(0,range,this.getValue());
      this.update();
   }
   function getRange()
   {
      return this.m_index.GetNumItems();
   }
   function setInfo(indexMin, indexMax, indexCurrent)
   {
      this.m_index.SetInfo(indexCurrent,indexMin,indexMax,1,false,false);
      this.update();
   }
   function enable()
   {
      this.visual_enableGUI();
   }
   function disable()
   {
      this.visual_disableGUI();
   }
   function highlight(id)
   {
      this.visual_highlight(id);
   }
   function unhighlight(id)
   {
      this.visual_unhighlight_clip(id);
   }
   function addListener(listener)
   {
      this.m_scrollbarEventListeners.push(listener);
   }
   function removeListener(listener)
   {
      while(this.removeAllInstancesOfListener(listener))
      {
      }
   }
   function removeAllInstancesOfListener(listener)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_scrollbarEventListeners.length)
      {
         if(this.m_scrollbarEventListeners[_loc2_] == listener)
         {
            this.m_scrollbarEventListeners[_loc2_] = null;
            this.m_scrollbarEventListeners.splice(_loc2_,1);
            return true;
         }
         _loc2_ = _loc2_ + 1;
      }
      return false;
   }
   function broadcastOnScrollbarDelta(dir)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_scrollbarEventListeners.length)
      {
         this.m_scrollbarEventListeners[_loc2_].OnScrollbarDelta(dir);
         _loc2_ = _loc2_ + 1;
      }
   }
   function broadcastOnScrollbarThumbUpdate(newValue)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_scrollbarEventListeners.length)
      {
         this.m_scrollbarEventListeners[_loc2_].OnScrollbarThumbUpdate(newValue);
         _loc2_ = _loc2_ + 1;
      }
   }
   function broadcastOnScrollbarMouseEnter()
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_scrollbarEventListeners.length)
      {
         this.m_scrollbarEventListeners[_loc2_].OnScrollbarMouseEnter();
         _loc2_ = _loc2_ + 1;
      }
   }
   function broadcastOnScrollbarMouseExit()
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_scrollbarEventListeners.length)
      {
         this.m_scrollbarEventListeners[_loc2_].OnScrollbarMouseExit();
         _loc2_ = _loc2_ + 1;
      }
   }
}
