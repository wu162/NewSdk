function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function onUnload()
{
   delete Move;
   delete onUnload;
}
if(!initialized)
{
   var labelX = productionLabelField._x;
   var margin = productionValueField._x - (productionLabelField._x + productionLabelField._width);
   var rightMargin = background._width - (productionValueField._x + productionValueField._width);
   productionLabelText = "$" + qualifyName(this,"ProductionLabel") + "&outline";
   productionLabelField.autoSize = "left";
   productionLabelField._width;
   productionValueText = "$" + qualifyName(this,"ProductionValue") + "&outline";
   productionValueField.autoSize = "left";
   productionValueField._width;
   consumptionLabelText = "$" + qualifyName(this,"ConsumptionLabel") + "&outline";
   consumptionLabelField.autoSize = "left";
   consumptionLabelField._width;
   consumptionValueText = "$" + qualifyName(this,"ConsumptionValue") + "&outline";
   consumptionValueField.autoSize = "left";
   consumptionValueField._width;
   var labelWidth = Math.max(productionLabelField._width,consumptionLabelField._width);
   productionLabelField._x = labelX + (labelWidth - productionLabelField._width);
   consumptionLabelField._x = labelX + (labelWidth - consumptionLabelField._width);
   var valueWidth = Math.max(productionValueField._width,consumptionValueField._width);
   var valueX = labelX + labelWidth + margin;
   productionValueField._x = valueX + (valueWidth - productionValueField._width);
   consumptionValueField._x = valueX + (valueWidth - consumptionValueField._width);
   background._width = valueX + valueWidth + rightMargin;
   background._visible = false;
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
stop();
