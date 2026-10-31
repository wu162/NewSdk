using eaf2apt.Characters;
using System;
using System.Collections.Generic;


using System.IO;
using SwfLib;
using SwfLib.Tags;
using SwfLib.Tags.BitmapTags;
using SwfLib.Tags.ButtonTags;
using SwfLib.Tags.ControlTags;
using SwfLib.Tags.FontTags;
using SwfLib.Tags.ShapeTags;
using SwfLib.Tags.ShapeMorphingTags;
using SwfLib.Tags.SoundTags;
using SwfLib.Tags.TextTags;
using SwfLib.Tags.VideoTags;


namespace eaf2apt
{
    enum ConstPointerType
    {
        imports = 0,
        exports = 1,
        characters = 2,
        maincharacter = 3
    };
    class AptConverter
    {
        Globals GlobalData;
        List<AptCharacterBitmap> bitmaps = new();
        List<AptCharacterShape> shapes = new();
        List<string> TextureIDs = new();
        List<string> NoPackedStrings = new();
        List<string> PackedStrings = new();
        List<string> ShapeIDs = new();

        private void PackTextures()
        {
            foreach (var bitmap in bitmaps)
            {
                var source = bitmap.Texture;
                if (bitmap.NoPack) { NoPackedStrings.Add($"{bitmap.ID}=0 0 {source.Width} {source.Height}"); bitmap.PackOffset = new FloatVector(); }
                else { PackedStrings.Add($"{bitmap.ID}->{bitmap.ID}"); bitmap.PackOffset = new FloatVector { X = 1, Y = 1 }; }
                int offset = bitmap.NoPack ? 0 : 1;
                int Pot(int n) { int result = 1; while (result <= n) result = checked(result * 2); return result; }
                int width = Pot(source.Width + offset * 2), height = Pot(source.Height + offset * 2);
                byte[] pixels = new byte[checked(width * height * 4)];
                for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
                {
                    if (!bitmap.NoPack && (x < offset || y < offset || x >= source.Width + offset || y >= source.Height + offset)) continue;
                    int sx = Math.Min(source.Width - 1, Math.Max(0, x - offset)), sy = Math.Min(source.Height - 1, Math.Max(0, y - offset));
                    Buffer.BlockCopy(source.Pixels, (sy * source.Width + sx) * 4, pixels, (y * width + x) * 4, 4);
                }
                GlobalData.Textures.Add(bitmap.ID, new MemoryTexture(width, height, pixels));
            }
        }
        void FindButtonHitTest()
        {
            for (int i = 0; i < GlobalData.AllCharacters.Length; i++)
            {
                if (GlobalData.AllCharacters[i] is AptCharacterButton button)
                {
                    foreach (var record in button.Tag.Characters)
                    {
                        if (record.StateDown || record.StateOver || record.StateUp)
                        {
                            GlobalData.ReferencedCharacters[record.CharacterID] = true;
                        }
                        if (record.StateHitTest)
                        {
                            GlobalData.HitTestShapes[record.CharacterID] = true;
                        }
                    }
                }
            }
        }
        internal MemoryMovie Convert(byte[] swfBytes)
        {
            var result = new MemoryMovie();
            GlobalData = new Globals();
            GlobalData.writer = new ActionWriter(GlobalData);
            using (var source = new MemoryStream(swfBytes))
            {
                var swf = SwfFile.ReadFrom(source);
                List<SwfTagBase> CharacterList = new();
                List<ImportAssetsBase> ImportAssetsList = new();
                List<ExportAssetsTag> ExportAssetsList = new();
                List<DefineTextTag> DefineTextList = new();
                List<DefineText2Tag> DefineText2List = new();
                Dictionary<int, DefineFontInfoBaseTag> DefineFontInfoList = new();
                Dictionary<int, DefineFontBaseTag> FontBaseList = new();
                foreach (var item in swf.Tags)
                {
                    if (!item.IsControlTag)
                    {
                        CharacterList.Add(item);
                    }
                    switch (item)
                    {
                        case ImportAssetsBase import:
                            ImportAssetsList.Add(import);
                            break;
                        case ExportAssetsTag export:
                            ExportAssetsList.Add(export);
                            break;
                        case DefineTextTag definetext:
                            DefineTextList.Add(definetext);
                            break;
                        case DefineText2Tag definetext2:
                            DefineText2List.Add(definetext2);
                            break;
                        case DefineFontInfoBaseTag definefontinfo:
                            DefineFontInfoList.Add(item.TagID, definefontinfo);
                            break;
                        case DefineFontBaseTag definefont:
                            FontBaseList.Add(item.TagID, definefont);
                            break;
                    }
                }
                foreach (var item in CharacterList)
                {
                    if (item is DefineSoundTag || item is DefineVideoStreamTag) throw new NotSupportedException("Embedded audio/video payloads are not supported by this RA3 UI asset converter.");
                    var id = item.TagID;
                    if (id > GlobalData.LastID)
                    {
                        GlobalData.LastID = id;
                    }
                }
                GlobalData.LastID++;
                foreach (var import in ImportAssetsList)
                {
                    foreach (var i in import.Tags)
                    {
                        if (i >= GlobalData.LastID)
                        {
                            GlobalData.LastID++;
                        }
                    }
                }

                Array.Resize(ref GlobalData.Characters, GlobalData.LastID);

                foreach (var text in DefineTextList)
                {
                    foreach (var i in text.TextRecords)
                    {
                        if (i.FontID is not null)
                        {
                            FontBaseList[i.FontID.Value].UsedInStaticText = true;
                        }
                    }
                }

                foreach (var text in DefineText2List)
                {
                    foreach (var i in text.TextRecords)
                    {
                        if (i.FontID is not null)
                        {
                            FontBaseList[i.FontID.Value].UsedInStaticText = true;
                        }
                    }
                }

                foreach (var item in CharacterList)
                {
                    switch (item)
                    {
                        case DefineBitsLosslessTag definebitslossless:
                            GlobalData.Characters[item.TagID] = new AptCharacterBitmap(definebitslossless, GlobalData);
                            break;
                        case DefineBitsLossless2Tag definebitslossless2:
                            GlobalData.Characters[item.TagID] = new AptCharacterBitmap(definebitslossless2, GlobalData);
                            break;
                        case JPEGTablesTag tables:
                            GlobalData.JPEGTables = tables.JPEGData;
                            break;
                        case DefineBitsTag definebits:
                            GlobalData.Characters[item.TagID] = new AptCharacterBitmap(definebits, GlobalData);
                            break;
                        case DefineBitsJPEG2Tag definebitsjpeg2:
                            GlobalData.Characters[item.TagID] = new AptCharacterBitmap(definebitsjpeg2, GlobalData);
                            break;
                        case DefineBitsJPEG3Tag definebitsjpeg3:
                            GlobalData.Characters[item.TagID] = new AptCharacterBitmap(definebitsjpeg3, GlobalData);
                            break;
                        case DefineButton2Tag definebutton2:
                            GlobalData.Characters[item.TagID] = new AptCharacterButton(definebutton2, GlobalData);
                            break;
                        case DefineFontTag definefont:
                            if (DefineFontInfoList.TryGetValue(item.TagID, out var fontinfo))
                            {
                                GlobalData.Characters[item.TagID] = new AptCharacterFont(definefont, fontinfo, GlobalData);
                            }
                            break;
                        case DefineFont2BaseTag definefont2:
                            GlobalData.Characters[item.TagID] = new AptCharacterFont(definefont2, GlobalData);
                            break;
                        case DefineMorphShapeTag definemorphshape:
                            GlobalData.Characters[item.TagID] = new AptCharacterMorph(definemorphshape, GlobalData);
                            break;
                        case DefineShapeTag defineshape:
                            GlobalData.Characters[item.TagID] = new AptCharacterShape(defineshape, GlobalData);
                            break;
                        case DefineShape2Tag defineshape2:
                            GlobalData.Characters[item.TagID] = new AptCharacterShape(defineshape2, GlobalData);
                            break;
                        case DefineShape3Tag defineshape3:
                            GlobalData.Characters[item.TagID] = new AptCharacterShape(defineshape3, GlobalData);
                            break;
                        case DefineSoundTag definesound:
                            GlobalData.Characters[item.TagID] = new AptCharacterSound(definesound, GlobalData);
                            break;
                        case DefineSpriteTag definesprite:
                            GlobalData.Characters[item.TagID] = new AptCharacterSprite(definesprite, GlobalData);
                            break;
                        case DefineTextTag definetext:
                            GlobalData.Characters[item.TagID] = new AptCharacterStaticText(definetext, GlobalData);
                            break;
                        case DefineText2Tag definetext2:
                            GlobalData.Characters[item.TagID] = new AptCharacterStaticText(definetext2, GlobalData);
                            break;
                        case DefineEditTextTag defineedittext:
                            GlobalData.Characters[item.TagID] = new AptCharacterText(defineedittext, GlobalData);
                            break;
                        case DefineVideoStreamTag definevideostream:
                            GlobalData.Characters[item.TagID] = new AptCharacterVideo(definevideostream, GlobalData);
                            break;
                    }
                }
                GlobalData.AllCharacters = new AptCharacter[GlobalData.Characters.Length + GlobalData.ExtraCharacters.Count];
                for (int i = 0; i < GlobalData.Characters.Length; i++)
                {
                    GlobalData.AllCharacters[i] = GlobalData.Characters[i];
                }
                for (int i = 0; i < GlobalData.ExtraCharacters.Count; i++)
                {
                    GlobalData.AllCharacters[GlobalData.Characters.Length + i] = GlobalData.ExtraCharacters[i];
                }
                GlobalData.movie = new AptMovie(swf.Tags, GlobalData);
                List<AptExport> exports = new();
                foreach (var e in ExportAssetsList)
                {
                    foreach (var s in e.Symbols)
                    {
                        exports.Add(new(s.SymbolID, s.SymbolName));
                        GlobalData.ReferencedCharacters[s.SymbolID] = true;
                    }
                }
                List<AptImport> imports = new();
                foreach (var imp in ImportAssetsList)
                {
                    for (int i = 0; i < imp.Tags.Count; i++)
                    {
                        imports.Add(new(imp.URL, imp.Names[i], imp.Tags[i]));
                    }
                }
                FindButtonHitTest();
                AptCharacterAnimation a;
                using (MemoryStream outfile = new MemoryStream())
                {
                    GlobalData.output.curpass = 0;
                    string tag = "Apt Data:" + (char)(swf.FileInfo.Version + 48) + "4\u001A\0";
                    GlobalData.output.SetInitialValue(tag);
                    GlobalData.output.Align(0);
                    int chars = GlobalData.AllCharacters.Length;
                    foreach (var i in imports)
                    {
                        if (i.ID > chars)
                        {
                            chars = i.ID + 1;
                        }
                    }
                    a = new AptCharacterAnimation(GlobalData.movie, GlobalData, 0, swf.Header.FrameRate, swf.Header.FrameSize, chars, imports, exports);
                    GlobalData.AllCharacters[0] = a;
                    for (int i = 0; i < chars; i++)
                    {
                        switch (GlobalData.AllCharacters[i])
                        {
                            case AptCharacterSprite sprite:
                                sprite.movie.AddConstants();
                                break;
                            case AptCharacterAnimation animation:
                                animation.movie.AddConstants();
                                break;
                            case AptCharacterBitmap bitmap:
                                bitmaps.Add(bitmap);
                                break;
                            case AptCharacterButton button:
                                foreach (var b in button.Tag.Conditions)
                                {
                                    GlobalData.writer.AddActionConstants(b.Actions);
                                }
                                button.UpdateHitTestShape();
                                break;
                            case AptCharacterShape shape:
                                shape.UpdateShape(i);
                                shapes.Add(shape);
                                break;
                        }
                    }
                    a.WriteImportsExports();
                    GlobalData.MaxPasses = 5;
                    for (GlobalData.output.curpass = 1; GlobalData.output.curpass <= GlobalData.MaxPasses; GlobalData.output.curpass++)
                    {
                        GlobalData.output.Align(1);
                        if (GlobalData.output.curpass == 1)
                        {
                            a.constpointers[ConstPointerType.characters] = GlobalData.output.GetSize();
                        }
                        if (GlobalData.output.curpass == 3)
                        {
                            a.constpointers[ConstPointerType.maincharacter] = GlobalData.output.GetSize();
                        }
                        for (int i = 0; i < chars; i++)
                        {
                            var ch = GlobalData.AllCharacters[i];
                            if (ch is null)
                            {
                                GlobalData.output.Write(2, 0);
                                continue;
                            }
                            ch.OutputCharacter(i);
                        }
                    }
                    result.Apt = GlobalData.output.GetBuffer();
                    GlobalData.output.curpass = 0;
                }
                using (MemoryStream outfile2 = new MemoryStream())
                {
                    GlobalData.output.SetInitialValue("Apt constant file\u001A\0");
                    GlobalData.MaxPasses = 1;
                    for (GlobalData.output.curpass = 1; GlobalData.output.curpass <= GlobalData.MaxPasses; GlobalData.output.curpass++)
                    {
                        GlobalData.output.Write(1, a.constpointers[ConstPointerType.maincharacter]);
                    }
                    GlobalData.writer.WriteConstTable();
                    result.Constants = GlobalData.output.GetBuffer();
                }
                PackTextures();
                foreach (var s in shapes)
                {
                    ShapeIDs.Add(s.outid.ToString());
                    result.Geometry.Add(s.outid, s.WriteShapeFile());
                }
                result.Dat = System.Text.Encoding.UTF8.GetBytes("; Created by AptToXML.\r\n" + string.Join("\r\n", NoPackedStrings) + (NoPackedStrings.Count > 0 ? "\r\n" : "") + "; Created by AptToXML.\r\n" + string.Join("\r\n", PackedStrings) + (PackedStrings.Count > 0 ? "\r\n" : ""));
                foreach (var texture in GlobalData.Textures) result.Textures.Add(texture.Key, texture.Value);
                return result;
            }
        }
    }
}
