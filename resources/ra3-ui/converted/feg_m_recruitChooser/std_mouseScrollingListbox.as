// WARNING: This class was decompiled without detecting uninitialized class fields.
class std_mouseScrollingListbox extends Cafe2_MouseBaseControl implements IListboxEntryEventListener, IScrollbarEventListener, ICafe2_FocusableControl
{
   static var ATTACHMOVIE_BASE_LEVEL = 500;
   static var NO_SELECTION = -1;
   function std_mouseScrollingListbox()
   {
      super();
      this.m_numTotalEntries = 0;
      this.m_contentWidth = 0;
      this.m_contentHeight = 0;
      this.m_entryHeight = 0;
      this.m_dataColumnCount = 0;
      this.m_selectedIndex = NO_SELECTION;
      this.m_scrollbarController = null;
      this.m_mousePresence = 0;
      this.m_eventListeners = new Array();
      this.m_eventListenerFns = new Array();
      this.m_scrollHeight = 0;
      this.m_useEntryHitRegion = true;
   }
   static function defaultInitObject()
   {
      var _loc1_ = new Cafe2_Imp_BaseControl.defaultInitObject();
      _loc1_.m_vPadding = 0;
      _loc1_.m_numVisibleEntries = 10;
      _loc1_.m_scrollbarOffset = Array([10,0]);
      _loc1_.m_backgroundMargin = 3;
      _loc1_.m_renderWidth = 0;
      _loc1_.m_renderHeight = 0;
      _loc1_.m_useHighlights = true;
      _loc1_.m_alternateBackgroundColors = false;
      _loc1_.m_backgroundColor1 = 0;
      _loc1_.m_backgroundColor2 = 0;
      return _loc1_;
   }
   static function createDynamic(container_, name_, level_, initObject_)
   {
      var _loc1_ = container_.attachMovie("std_mouseScrollingListboxSymbol",name_,level_,initObject_);
      return std_mouseScrollingListbox(_loc1_);
   }
   function enableGUI()
   {
   }
   function disableGUI()
   {
   }
   function highlight()
   {
      if(this.isEnabled() && this._visible && this.m_useHighlights)
      {
         this.background.gotoAndPlay("_highlight");
      }
   }
   function unhighlight()
   {
      if(this.isEnabled() && this._visible && this.m_useHighlights)
      {
         this.background.gotoAndPlay("_unhighlight");
      }
   }
   function OnScrollbarDelta(dir)
   {
      switch(dir)
      {
         case 0:
            this.scrollViewUp();
            break;
         case 1:
            this.scrollViewDown();
         default:
            return;
      }
   }
   function OnScrollbarThumbUpdate(percentage)
   {
      if(this.m_contentHeight > this.m_renderHeight)
      {
         this.m_scrollHeight = percentage / 100 * (this.m_contentHeight - this.m_renderHeight);
         this.m_clipAnchor._x = 0;
         this.m_clipAnchor._y = - this.m_scrollHeight;
      }
   }
   function OnScrollbarMouseEnter()
   {
   }
   function OnScrollbarMouseExit()
   {
   }
   function OnListboxEntryMouseEnter(entry)
   {
      if(this.isEnabled())
      {
         if(entry >= 0 && entry < this.m_numTotalEntries)
         {
            this.m_entryClips[entry].realizeHighlightState();
         }
      }
      if(this._ymouse < 0 || this._ymouse > this.m_renderHeight || this._xmouse < 0 || this._xmouse > this.m_renderWidth || entry >= this.getGreatestColumnDataCount())
      {
         return undefined;
      }
      this.OnChildMouseEnterEvent(entry);
   }
   function OnListboxEntryMouseExit(entry)
   {
      if(this.isEnabled())
      {
         if(entry >= 0 && entry < this.m_numTotalEntries)
         {
            this.m_entryClips[entry].realizeHighlightState();
         }
      }
      this.OnChildMouseExitEvent(entry);
   }
   function OnListboxEntryMouseClick(entry)
   {
      this.m_objFM.setFocusToControl(this);
      var _loc3_;
      var _loc5_;
      if(this.isEnabled())
      {
         _loc3_ = this.getGreatestColumnDataCount();
         if(this._ymouse < 0 || this._ymouse > this.m_renderHeight || this._xmouse < 0 || this._xmouse > this.m_renderWidth || entry >= _loc3_)
         {
            return undefined;
         }
         _loc5_ = this.getSelectedEntryIndex();
         this.setSelectedIndex(entry);
         this.broadcastOnListboxIndexMouseClick(entry,_loc5_);
         _global.playSound("GuiShellListboxClick");
      }
      else
      {
         this.m_objFM.deselectFocusedComponent();
      }
   }
   function OnListboxEntryElementMouseClick(entry, element)
   {
      var _loc3_;
      var _loc5_;
      if(this.isEnabled())
      {
         _loc3_ = this.getGreatestColumnDataCount();
         if(this._ymouse < 0 || this._ymouse > this.m_renderHeight || this._xmouse < 0 || this._xmouse > this.m_renderWidth || entry >= _loc3_)
         {
            return undefined;
         }
         _loc5_ = this.getSelectedEntryIndex();
         this.setSelectedIndex(entry);
         this.broadcastOnListboxIndexElementMouseClick(entry,element,_loc5_);
         _global.playSound("GuiShellListboxClick");
      }
      else
      {
         this.m_objFM.deselectFocusedComponent();
      }
   }
   function onMousewheelScroll(spins)
   {
      var _loc3_ = Math.abs(spins);
      var _loc2_ = 0;
      while(_loc2_ < _loc3_)
      {
         if(spins < 0)
         {
            this.scrollViewDown();
         }
         else
         {
            this.scrollViewUp();
         }
         _loc2_ = _loc2_ + 1;
      }
   }
   function init(entryElementInfo, entryHeight)
   {
      if(this.m_numVisibleEntries == undefined || this.m_numVisibleEntries == 0)
      {
         this.m_numVisibleEntries = 8;
      }
      this.m_entryClips = new Array();
      this.m_scrollHeight = 0;
      this.m_contentHeight = this.m_numVisibleEntries * entryHeight;
      this.m_contentWidth = 0;
      this.m_entryElementInfo = entryElementInfo;
      this.m_entryHeight = entryHeight;
      var _loc2_ = 0;
      while(_loc2_ < entryElementInfo.length)
      {
         this.m_contentWidth += entryElementInfo[_loc2_].width;
         this.m_contentWidth += entryElementInfo[_loc2_].leftMargin;
         this.m_contentWidth += entryElementInfo[_loc2_].rightMargin;
         _loc2_ = _loc2_ + 1;
      }
      if(this.m_renderWidth == 0 || this.m_renderWidth == undefined || this.m_renderHeight == 0 || this.m_renderHeight == undefined)
      {
         trace("ERROR: Bad render sizes specified. Using defaults.");
         this.m_renderWidth = this.m_contentWidth;
         this.m_renderHeight = this.m_contentHeight;
         trace("Render width: " + this.m_renderWidth + " Render height: " + this.m_renderHeight);
      }
      if(this.m_backgroundMargin == undefined)
      {
         this.m_backgroundMargin = 3;
      }
      this.m_dataColumnCount = this.m_entryElementInfo.length;
      this.m_dataArrays = new Array(this.m_dataColumnCount);
      _loc2_ = 0;
      while(_loc2_ < this.m_dataColumnCount)
      {
         this.m_dataArrays[_loc2_] = new Array();
         _loc2_ = _loc2_ + 1;
      }
      this.configureBackgroundButton();
      this.background._visible = this.m_BgVisible;
      this.m_listboxMask._width = this.getWidth();
      this.m_listboxMask._height = this.getHeight();
      this.m_listboxMask.onRollOver = this.m_listboxMask.onDragOver = this.bind0(this,this.OnMouseEnter);
      this.m_listboxMask.onRollOut = this.m_listboxMask.onDragOut = this.bind0(this,this.OnMouseExit);
      if(this.m_alternateBackgroundColors == true)
      {
         this.m_contentHeight = 0;
         _loc2_ = 0;
         while(_loc2_ < this.m_numVisibleEntries)
         {
            this.m_entryClips[_loc2_] = this.createEntryClip(_loc2_,0,this.m_contentHeight,this.m_entryElementInfo,this.m_entryHeight);
            this.m_entryClips[_loc2_]._y = this.m_contentHeight;
            this.m_contentHeight += this.m_entryHeight + this.m_vPadding;
            _loc2_ = _loc2_ + 1;
         }
      }
      this.realizeHighlightState();
      this.commonInit();
   }
   function refreshDisplay()
   {
      var _loc6_ = this.getGreatestColumnDataCount();
      var _loc7_ = this.m_entryClips.length != undefined ? this.m_entryClips.length : 0;
      this.m_numTotalEntries = Math.max(_loc6_,this.m_numVisibleEntries);
      var _loc8_ = 100 * this.m_scrollHeight / (this.m_contentHeight - this.m_renderHeight);
      this.m_contentHeight = 0;
      var _loc2_;
      _loc2_ = 0;
      var _loc3_;
      var _loc5_;
      var _loc4_;
      while(_loc2_ < this.m_numTotalEntries)
      {
         if(_loc2_ >= _loc7_)
         {
            this.m_entryClips[_loc2_] = this.createEntryClip(_loc2_,0,this.m_contentHeight,this.m_entryElementInfo,this.m_entryHeight);
         }
         this.m_entryClips[_loc2_].clear();
         _loc3_ = 0;
         while(_loc2_ < _loc6_ && _loc3_ < this.m_dataColumnCount)
         {
            _loc5_ = this.m_dataArrays[_loc3_];
            _loc4_ = new Object();
            _loc4_.data = _loc5_[_loc2_];
            this.m_entryClips[_loc2_].setElementData(_loc3_,_loc4_);
            _loc3_ = _loc3_ + 1;
         }
         this.m_entryClips[_loc2_].setSelected(_loc2_ == this.getSelectedEntryIndex());
         this.m_entryClips[_loc2_]._y = this.m_contentHeight;
         this.m_contentHeight += this.m_entryClips[_loc2_].getHeight() + this.m_vPadding;
         _loc2_ = _loc2_ + 1;
      }
      _loc2_ = _loc7_ - 1;
      while(_loc2_ >= this.m_numTotalEntries)
      {
         this.m_entryClips[_loc2_].removeMovieClip();
         this.m_entryClips.pop();
         _loc2_ = _loc2_ - 1;
      }
      this.OnScrollbarThumbUpdate(_loc8_);
      this.m_clipAnchor._x = 0;
      this.m_clipAnchor._y = - this.m_scrollHeight;
   }
   function shutdown()
   {
      if(this.m_scrollbarController != null)
      {
         this.m_scrollbarController.removeListener(this);
      }
      var _loc2_ = 0;
      while(_loc2_ < this.m_numTotalEntries)
      {
         this.m_entryClips[_loc2_].removeListener(this);
         this.m_entryClips[_loc2_].shutdown();
         _loc2_ = _loc2_ + 1;
      }
   }
   function setColumnData(column, data)
   {
      var _loc4_;
      var _loc2_;
      if(column >= 0 && column < this.m_dataColumnCount)
      {
         _loc4_ = this.m_dataArrays[column];
         _loc4_.splice(0,_loc4_.length);
         _loc2_ = 0;
         while(_loc2_ < data.length)
         {
            _loc4_[_loc2_] = data[_loc2_];
            _loc2_ = _loc2_ + 1;
         }
      }
   }
   function clear()
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_numTotalEntries)
      {
         this.m_entryClips[_loc2_].removeListener(this);
         this.m_entryClips[_loc2_].clear();
         this.m_entryClips[_loc2_].shutdown();
         this.m_entryClips[_loc2_].removeMovieClip();
         _loc2_ = _loc2_ + 1;
      }
      this.m_entryClips = new Array();
      this.m_numTotalEntries = 0;
      this.m_scrollHeight = this.m_contentHeight = 0;
      this.refreshScrollbar();
      var _loc3_ = 0;
      while(_loc3_ < this.m_dataColumnCount)
      {
         this.m_dataArrays[_loc3_] = new Array();
         _loc3_ = _loc3_ + 1;
      }
   }
   function clearEntryHighlights()
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_numTotalEntries)
      {
         this.m_entryClips[_loc2_].setHighlighted(false);
         _loc2_ = _loc2_ + 1;
      }
   }
   function attachScrollbarController(controller)
   {
      if(this.m_scrollbarController != null)
      {
         trace("std_mouseDynamicList::attachScrollbarController(), Warning: trying to attach a scrollbar controller when one is already attached.");
         this.m_scrollbarController.removeListener(this);
      }
      this.m_scrollbarController = std_mouseScrollbar_visual(controller);
      this.m_scrollbarController.addListener(this);
      this.m_scrollbarController._x = this._x + this.m_renderWidth - this.m_scrollbarController.getWidth();
      this.m_scrollbarController._y = this._y;
      this.m_scrollbarController.setSize(this.m_renderHeight);
      this.m_scrollbarController.enable();
   }
   function getElementData(column, row)
   {
      return this.m_dataArrays[column][row];
   }
   function refreshScrollbar()
   {
      if(this.m_scrollbarController != undefined)
      {
         if(this.m_contentHeight <= this.m_renderHeight)
         {
            this.m_scrollbarController._visible = false;
            this.m_scrollbarController.setInfo(0,0,0);
            this.m_scrollHeight = 0;
         }
         else
         {
            this.m_scrollbarController._visible = true;
            this.m_scrollbarController.setInfo(0,this.m_contentHeight - this.m_renderHeight,this.m_scrollHeight);
         }
      }
   }
   function setSelectedIndex(index)
   {
      if(this.isValidIndex(index) || index == NO_SELECTION)
      {
         this.m_selectedIndex = index;
         this.refreshDisplay();
      }
      return this.m_selectedIndex;
   }
   function scrollViewUp()
   {
      if(this.m_selectedIndex != NO_SELECTION)
      {
         this.m_scrollHeight -= this.m_entryClips[this.m_selectedIndex].getHeight() + this.m_vPadding;
      }
      else
      {
         this.m_scrollHeight -= 10;
      }
      if(this.m_scrollHeight < 0)
      {
         this.m_scrollHeight = 0;
      }
      this.m_clipAnchor._x = 0;
      this.m_clipAnchor._y = - this.m_scrollHeight;
      this.refreshScrollbar();
   }
   function scrollViewDown()
   {
      if(this.m_scrollHeight == undefined)
      {
         this.m_scrollHeight = 0;
      }
      if(this.m_selectedIndex != NO_SELECTION)
      {
         this.m_scrollHeight += this.m_entryClips[this.m_selectedIndex].getHeight() + this.m_vPadding;
      }
      else
      {
         this.m_scrollHeight += 10;
      }
      if(this.m_scrollHeight > this.m_contentHeight - this.m_renderHeight)
      {
         this.m_scrollHeight = this.m_contentHeight - this.m_renderHeight;
      }
      this.m_clipAnchor._x = 0;
      this.m_clipAnchor._y = - this.m_scrollHeight;
      this.refreshScrollbar();
   }
   function moveViewToFirst()
   {
      this.m_scrollHeight = 0;
      this.m_clipAnchor._x = 0;
      this.m_clipAnchor._y = - this.m_scrollHeight;
      this.refreshScrollbar();
   }
   function moveViewToLast()
   {
      if(this.m_contentHeight > this.m_renderHeight)
      {
         this.m_scrollHeight = this.m_contentHeight - this.m_renderHeight;
         this.m_clipAnchor._x = 0;
         this.m_clipAnchor._y = - this.m_scrollHeight;
         this.refreshScrollbar();
      }
   }
   function setNumEntries(numEntries)
   {
      this.m_numVisibleEntries = numEntries;
   }
   function getNumEntries(numEntries)
   {
      return this.m_numVisibleEntries;
   }
   function setVerticalPadding(vPadding)
   {
      this.m_vPadding = vPadding;
   }
   function getTotalWidth()
   {
      return this.background._width;
   }
   function getWidth()
   {
      return this.m_renderWidth;
   }
   function getHeight()
   {
      return this.m_renderHeight;
   }
   function setRenderSize(renderHeight, renderWidth)
   {
      this.m_renderHeight = renderHeight;
      this.m_renderWidth = renderWidth;
   }
   function getSelectedEntryIndex()
   {
      return this.m_selectedIndex;
   }
   function disableEntryHitRegion()
   {
      this.m_useEntryHitRegion = false;
   }
   function addListener(listener)
   {
      this.m_eventListeners.push(listener);
   }
   function addListenerFunction(listener)
   {
      this.m_eventListenerFns.push(listener);
   }
   function removeListener(listener)
   {
      while(this.removeAllInstancesOfListener(listener))
      {
      }
   }
   function removeListenerFunction(listener)
   {
      while(this.removeAllInstancesOfListenerFunction(listener))
      {
      }
   }
   function createEntryClip(index, xPos, yPos, entryElementInfo, entryHeight)
   {
      var _loc2_ = this.m_clipAnchor.attachMovie("std_mouseScrollingListboxEntrySymbol","listboxEntity" + String(index),ATTACHMOVIE_BASE_LEVEL + index);
      var _loc3_;
      if(this.m_alternateBackgroundColors)
      {
         if(index % 2 == 0)
         {
            _loc3_ = this.m_backgroundColor1;
         }
         else
         {
            _loc3_ = this.m_backgroundColor2;
         }
         _loc2_.setBackgroundColor(_loc3_);
      }
      _loc2_.init(index,entryElementInfo,entryHeight,this.m_useEntryHitRegion,this.m_alternateBackgroundColors);
      _loc2_._x = xPos;
      _loc2_._y = yPos;
      _loc2_.addListener(this);
      return _loc2_;
   }
   function OnChildMouseEnterEvent(entry)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_eventListeners.length)
      {
         this.m_eventListeners[_loc2_].OnListboxEntryMouseEnter(entry);
         _loc2_ = _loc2_ + 1;
      }
   }
   function OnChildMouseExitEvent(entry)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_eventListeners.length)
      {
         this.m_eventListeners[_loc2_].OnListboxEntryMouseExit(entry);
         _loc2_ = _loc2_ + 1;
      }
   }
   function OnMouseEnter()
   {
      this.m_mousePresence = this.m_mousePresence + 1;
      _global.highlightedComponent = this;
      this.realizeHighlightState();
   }
   function OnMouseExit()
   {
      this.m_mousePresence = this.m_mousePresence - 1;
      if(_global.highlightedComponent == this)
      {
         _global.highlightedComponent = undefined;
      }
      if(this.m_mousePresence == 0)
      {
         this.realizeHighlightState();
      }
   }
   function configureBackgroundButton()
   {
      var _loc2_ = this.m_renderWidth + this.m_backgroundMargin * 2;
      var _loc3_ = this.m_renderHeight + this.m_backgroundMargin * 2;
      this.background._xscale = _loc2_;
      this.background._yscale = _loc3_;
      this.background._x = - this.m_backgroundMargin;
      this.background._y = - this.m_backgroundMargin;
   }
   function realizeHighlightState()
   {
      if(this.m_mousePresence > 0)
      {
         this.highlight();
      }
      else
      {
         this.unhighlight();
         this.m_mousePresence = 0;
      }
   }
   function getMaximumIndex()
   {
      return this.getGreatestColumnDataCount() - 1;
   }
   function getGreatestColumnDataCount()
   {
      var _loc3_ = 0;
      var _loc2_ = 0;
      while(_loc2_ < this.m_dataColumnCount)
      {
         _loc3_ = Math.max(_loc3_,this.m_dataArrays[_loc2_].length);
         _loc2_ = _loc2_ + 1;
      }
      return _loc3_;
   }
   function isValidIndex(index)
   {
      return index >= 0 && index <= this.getMaximumIndex();
   }
   function removeAllInstancesOfListener(listener)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_eventListeners.length)
      {
         if(this.m_eventListeners[_loc2_] == listener)
         {
            this.m_eventListeners[_loc2_] = null;
            this.m_eventListeners.splice(_loc2_,1);
            return true;
         }
         _loc2_ = _loc2_ + 1;
      }
      return false;
   }
   function removeAllInstancesOfListenerFunction(listener)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_eventListenerFns.length)
      {
         if(this.m_eventListenerFns[_loc2_] == listener)
         {
            this.m_eventListenerFns[_loc2_] = null;
            this.m_eventListenerFns.splice(_loc2_,1);
            return true;
         }
         _loc2_ = _loc2_ + 1;
      }
      return false;
   }
   function broadcastOnListboxIndexMouseClick(newIndex, previouslySelectedIndex)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_eventListeners.length)
      {
         this.m_eventListeners[_loc2_].OnListboxIndexMouseClick(newIndex,previouslySelectedIndex);
         _loc2_ = _loc2_ + 1;
      }
      _loc2_ = 0;
      while(_loc2_ < this.m_eventListenerFns.length)
      {
         this.m_eventListenerFns[_loc2_](newIndex,previouslySelectedIndex);
         _loc2_ = _loc2_ + 1;
      }
   }
   function broadcastOnListboxIndexElementMouseClick(index, element, previouslySelectedIndex)
   {
      var _loc2_ = 0;
      while(_loc2_ < this.m_eventListeners.length)
      {
         this.m_eventListeners[_loc2_].OnListboxIndexElementMouseClick(index,element,previouslySelectedIndex);
         _loc2_ = _loc2_ + 1;
      }
   }
}
