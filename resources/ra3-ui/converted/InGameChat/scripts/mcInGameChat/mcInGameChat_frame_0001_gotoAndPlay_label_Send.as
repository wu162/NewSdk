Send.buttonName = "$Send";
Cancel.buttonName = "$Cancel";
Send.DoButtonCallBack = function()
{
   Send.gotoAndPlay("_up");
   _root.GameCode("Send");
};
Cancel.DoButtonCallBack = function()
{
   Cancel.gotoAndPlay("_disabled");
   _root.GameCode("Close");
   trace("Closing" + _global.LabelToClose);
   _root.CloseChat();
};
Release = function(param)
{
   eval(param).gotoAndPlay("_down");
   _root.PlaySound("Gui_ShellMapSelect");
};
Press = function(param)
{
   eval(param).gotoAndStop("_down");
};
RollOver = function(param)
{
   eval(param).gotoAndPlay("_over");
   _root.PlaySound("Gui_ShellMapMouseOver");
};
RollOut = function(param)
{
   eval(param).gotoAndPlay("_rollout");
   _root.PlaySound("Gui_ShellMapMouseOut");
};
stop();
