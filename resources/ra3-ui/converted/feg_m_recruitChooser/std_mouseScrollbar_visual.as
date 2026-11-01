// WARNING: This class was decompiled without detecting uninitialized class fields.
class std_mouseScrollbar_visual extends std_mouseScrollbar_logic
{
   var m_scrollDirection = 0;
   var m_scrollArrowHeight = 0;
   function std_mouseScrollbar_visual()
   {
      super();
   }
   static function defaultInitObject()
   {
      var _loc1_ = std_mouseScrollbar_logic.defaultInitObject();
      _loc1_.m_nLength = 100;
      return _loc1_;
   }
   static function createDynamic(container_, name_, level_, initObject_)
   {
      return std_mouseScrollbar_visual(container_.attachMovie("std_scrollbar",name_,level_,initObject_));
   }
   function onLoad()
   {
      this.setSize(this.m_nLength);
      this._parent.onControlLoaded(this);
      this.m_scrollArrowHeight = this.arrowTopMC._height;
      this.arrowBottomMC.hitRegion.mComponentPtr = this;
      this.arrowBottomMC.hitRegion.onRollOver = this.mouseEventDecScrollBarRollOver;
      this.arrowBottomMC.hitRegion.onRollOut = this.mouseEventDecScrollBarRollOut;
      this.arrowBottomMC.hitRegion.onPress = this.downArrowClicked;
      this.arrowBottomMC.hitRegion.onRelease = this.stopTrackArrow;
      this.arrowBottomMC.hitRegion.onReleaseOutside = this.mouseEventDecScrollBarRollOut;
      this.arrowTopMC.hitRegion.mComponentPtr = this;
      this.arrowTopMC.hitRegion.onRollOver = this.mouseEventIncScrollBarRollOver;
      this.arrowTopMC.hitRegion.onRollOut = this.mouseEventIncScrollBarRollOut;
      this.arrowTopMC.hitRegion.onPress = this.upArrowClicked;
      this.arrowTopMC.hitRegion.onRelease = this.stopTrackArrow;
      this.arrowTopMC.hitRegion.onReleaseOutside = this.mouseEventIncScrollBarRollOut;
      this.thumbMC.hitRegion.mComponentPtr = this;
      this.thumbMC.hitRegion.onRollOver = this.mouseEventThumbRollOver;
      this.thumbMC.hitRegion.onRollOut = this.mouseEventThumbRollOut;
      this.thumbMC.hitRegion.onDragOver = this.mouseEventThumbDragOver;
      this.thumbMC.hitRegion.onDragOut = this.mouseEventThumbDragOut;
      this.thumbMC.hitRegion.onPress = this.mouseEventThumbMouseDown;
      this.thumbMC.hitRegion.onRelease = this.mouseEventThumbMouseUp;
      this.thumbMC.hitRegion.onReleaseOutside = this.mouseEventThumbMouseUpReset;
      this.trackMiddle.hitRegion.mComponentPtr = this;
      this.trackMiddle.hitRegion.onPress = this.mouseEventTrackMouseDown;
      this.trackMiddle.hitRegion.onRelease = this.mouseEventTrackMouseUp;
      this.trackMiddle.hitRegion.onReleaseOutside = this.mouseEventTrackMouseUp;
   }
   function setSize(p_nSize)
   {
      this.THUMB_MARGIN = this.arrowTopMC._height;
      this.m_nLength = p_nSize;
      this.m_thumbZoneLength = this.m_nLength - this.arrowTopMC._height - this.arrowBottomMC._height - 2 * this.THUMB_MARGIN;
      this.arrowBottomMC._y = this.m_nLength - this.arrowBottomMC._height;
      this.TrackBottom._y = this.arrowBottomMC._y - this.TrackBottom._height;
      this.TrackMiddle._y = this.TrackTop._y + this.TrackTop._height;
      this.TrackMiddle._height = this.TrackBottom._y - this.TrackTop._y - this.TrackTop._height;
      this.m_minThumbCoordinate = this.arrowTopMC._y + this.arrowTopMC._height + this.THUMB_MARGIN;
      this.m_maxThumbCoordinate = this.m_minThumbCoordinate + this.m_thumbZoneLength;
      this.updateVisuals();
   }
   function getWidth()
   {
      return this.TrackMiddle._width;
   }
   function updateVisuals()
   {
      var _loc2_ = this.m_index.GetMaxIndex() - this.m_index.GetMinIndex();
      var _loc3_ = _loc2_ != 0 ? this.m_thumbZoneLength / _loc2_ : this.m_thumbZoneLength;
      this.thumbMC._y = this.m_scrollArrowHeight + this.THUMB_MARGIN + Math.floor(this.m_index.GetIndex() * _loc3_);
   }
   function update()
   {
      this.updateVisuals();
   }
   function updateIndexNormalized(normalizedPct)
   {
      var _loc2_ = this.m_index.GetMinIndex() + Math.round(this.m_index.GetNumItems() * normalizedPct);
      this.m_index.SetIndex(_loc2_);
   }
   function onNext()
   {
      this.arrowBottomMC.gotoAndPlay("_toggle");
   }
   function onPrev()
   {
      this.arrowTopMC.gotoAndPlay("_toggle");
   }
   function visual_highlight(id)
   {
      switch(id)
      {
         case std_mouseScrollbar_logic.CONTROL_THUMB:
            this.thumbMC.gotoAndPlay("_highlight");
            break;
         case std_mouseScrollbar_logic.CONTROL_INCREMENT_ARROW:
            this.arrowTopMC.gotoAndPlay("_highlight");
            break;
         case std_mouseScrollbar_logic.CONTROL_DECREMENT_ARROW:
            this.arrowBottomMC.gotoAndPlay("_highlight");
      }
      this.lineTopMC.gotoAndPlay("_highlight");
      this.lineBottomMC.gotoAndPlay("_highlight");
   }
   function visual_unhighlight_clip(id)
   {
      switch(id)
      {
         case std_mouseScrollbar_logic.CONTROL_THUMB:
            this.thumbMC.gotoAndPlay("_unhighlight");
            break;
         case std_mouseScrollbar_logic.CONTROL_INCREMENT_ARROW:
            this.arrowTopMC.gotoAndPlay("_unhighlight");
            break;
         case std_mouseScrollbar_logic.CONTROL_DECREMENT_ARROW:
            this.arrowBottomMC.gotoAndPlay("_unhighlight");
         default:
            return;
      }
   }
   function visual_unhighlight()
   {
      this.arrowTopMC.gotoAndPlay("_unhighlight");
      this.arrowBottomMC.gotoAndPlay("_unhighlight");
      this.thumbMC.gotoAndPlay("_unhighlight");
      this.lineTopMC.gotoAndPlay("_unhighlight");
      this.lineBottomMC.gotoAndPlay("_unhighlight");
   }
   function visual_enableGUI()
   {
      this.visual_unhighlight();
   }
   function visual_disableGUI()
   {
      this.arrowTopMC.gotoAndPlay("_disable");
      this.arrowBottomMC.gotoAndPlay("_disable");
      this.thumbMC.gotoAndPlay("_disable");
      this.lineTopMC.gotoAndPlay("_disable");
      this.lineBottomMC.gotoAndPlay("_disable");
   }
   function rollover(id)
   {
      this.highlight(id);
      this.broadcastOnScrollbarMouseEnter();
   }
   function rollout(id)
   {
      this.unhighlight(id);
      this.broadcastOnScrollbarMouseExit();
   }
   function dragOver()
   {
   }
   function dragOut()
   {
   }
   function prepareToDragThumbTool()
   {
      this.m_DragThumbToolb = true;
      this.onEnterFrame = this.trackThumbTool;
   }
   function endToDragThumbTool()
   {
      this.m_DragThumbToolb = false;
      this.onEnterFrame = null;
      this.updateVisuals();
   }
   function trackThumbTool()
   {
      var _loc2_ = Math.min(Math.max(this._ymouse,this.m_minThumbCoordinate),this.m_maxThumbCoordinate);
      var _loc3_;
      var _loc4_;
      if(_loc2_ != this.thumbMC._y)
      {
         _loc3_ = (_loc2_ - this.m_minThumbCoordinate) / (this.m_maxThumbCoordinate - this.m_minThumbCoordinate);
         _loc4_ = 100 * _loc3_;
         this.thumbMC._y = _loc2_;
         this.updateIndexNormalized(_loc3_);
         this.broadcastOnScrollbarThumbUpdate(_loc4_);
      }
   }
   function updateData(dataChangeDirection)
   {
      this.broadcastOnScrollbarDelta(dataChangeDirection);
      if(dataChangeDirection == 0)
      {
         this.prevValue();
      }
      else
      {
         this.nextValue();
      }
   }
   function show()
   {
      this._visible = true;
   }
   function hide()
   {
      this._visible = false;
   }
   function mouseEventDecScrollBarRollOver()
   {
      this.mComponentPtr.rollover(std_mouseScrollbar_logic.CONTROL_DECREMENT_ARROW);
   }
   function mouseEventDecScrollBarRollOut()
   {
      this.mComponentPtr.rollout(std_mouseScrollbar_logic.CONTROL_DECREMENT_ARROW);
   }
   function mouseEventIncScrollBarRollOver()
   {
      this.mComponentPtr.rollover(std_mouseScrollbar_logic.CONTROL_INCREMENT_ARROW);
   }
   function mouseEventIncScrollBarRollOut()
   {
      this.stopTrackArrow();
      this.mComponentPtr.rollout(std_mouseScrollbar_logic.CONTROL_INCREMENT_ARROW);
   }
   function mouseEventThumbRollOver()
   {
      this.mComponentPtr.rollover(std_mouseScrollbar_logic.CONTROL_THUMB);
   }
   function mouseEventThumbRollOut()
   {
      this.mComponentPtr.rollout(std_mouseScrollbar_logic.CONTROL_THUMB);
   }
   function mouseEventThumbDragOver()
   {
      this.mComponentPtr.dragOver();
   }
   function mouseEventThumbDragOut()
   {
      this.mComponentPtr.dragOut();
   }
   function mouseEventThumbMouseDown()
   {
      this.mComponentPtr.prepareToDragThumbTool();
   }
   function mouseEventThumbMouseUp()
   {
      this.mComponentPtr.endToDragThumbTool();
   }
   function mouseEventThumbMouseUpReset()
   {
      this.mComponentPtr.endToDragThumbTool();
      this.mComponentPtr.rollout(std_mouseScrollbar_logic.CONTROL_THUMB);
   }
   function mouseEventTrackMouseDown()
   {
      this.mComponentPtr.prepareToDragThumbTool();
   }
   function mouseEventTrackMouseUp()
   {
      this.mComponentPtr.endToDragThumbTool();
   }
   function upArrowClicked()
   {
      this.mComponentPtr.prepareArrowClicked(0);
   }
   function downArrowClicked()
   {
      this.mComponentPtr.prepareArrowClicked(1);
   }
   function prepareArrowClicked(direction)
   {
      this.m_scrollDirection = direction;
      this.onEnterFrame = this.trackArrowTool;
   }
   function endToArrowClicked()
   {
      this.onEnterFrame = null;
      this.updateVisuals();
   }
   function trackArrowTool()
   {
      this.updateData(this.m_scrollDirection);
   }
   function stopTrackArrow()
   {
      this.mComponentPtr.endToArrowClicked();
   }
}
