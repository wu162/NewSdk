barSize = bar._width;
barScaler = targetSize - barSize;
bar._width = barSize + barScaler / easeFactor;
