//========================================================//
//===== ABOUT ============================================//
//========================================================//
//                                                        //
// COMMAND & CONQUER RED ALERT 3 MOD SDK - ASSET RESOLVER //
//                                                        //
// Author: Bibber                                         //
// eMail: m.bibber@web.de                                 //
// Homepage: http://bibber.bplaced.net                    //
//                                                        //
//========================================================//
//===== PROGRAM ==========================================//
//========================================================//

program ModAssetResolver;

{$APPTYPE CONSOLE}

uses
  SysUtils, Windows, ActiveX, XMLintf, XMLDoc, Registry;

// Types
type
  TManifestHeader = record
    BigEndian, Linked: Boolean;
    Version: Word;
	  StreamChecksum, AllTypesHash, AssetCount, TotalInstanceDataSize, MaxInstanceChunkSize, MaxRelocationChunkSize, MaxImportsChunkSize, AssetReferenceBufferSize, ReferencedManifestNameBufferSize, AssetNameBufferSize, SourceFileNameBufferSize: LongWord;
  end;

  TManifestAsset = record
    TypeID, InstanceID, TypeHash, InstanceHash, AssetReferenceOffset, AssetReferenceCount, NameOffset, SourceFileNameOffset, InstanceDataSize, RelocationDataSize, ImportsDataSize, Unknown: LongWord;
  end;

  TAssetReference = record
    TypeID, InstanceID: LongWord;
  end;

  TTypeInstance = record
    TypeID, InstanceID: LongWord;
    TypeName, InstanceName: WideString;
  end;

  TBigHeader = record
    BigType: array[0..3] of AnsiChar;
    BigSize, FileCount, DataOffset: LongWord;
  end;

  TBigEntry = record
    EntryOffset, EntrySize: LongWord;
  end;

// Constants
const
  ManifestHeaderSize = $30;
  ManifestAssetSize = $30;
  ManifestAssetReferenceSize = $08;

// Variables
var
  ModAssetList, AudioAssetList, GlobalAssetList, StaticAssetList, WorldbuilderAssetList, InsertAssetList: array of TManifestAsset;
  WorldbuilderFile, WorldbuilderBinFile, WorldbuilderImpFile, WorldbuilderReloFile: File;
  WorldbuilderHeader: TManifestHeader;
  InsertAssetReferenceBuffer: array of TAssetReference;
  DanglingAssets: array of TTypeInstance;
  InsertAssetReferenceOffset, InsertNameOffset, InsertSourceFileNameOffset: LongWord;
  InsertBin, InsertImp, InsertRelo, InsertNameBuffer, InsertSourceFileNameBuffer: AnsiString;
  ManifestPath, WorldbuilderBig, StringHashesXml: WideString;
  LineWritten: Boolean;



// Functions + procedures
function SwapDWord(DWord: LongWord): LongWord;
asm
  bswap eax;
end;



function GetError(E: Exception): WideString;
var
  EIOError: EInOutError;
begin
  if (E.ClassName = 'EInOutError') then begin
    EIOError := EInOutError(E);
    Result := 'I/O Error '+IntToStr(EIOError.ErrorCode)+': '+SysErrorMessage(EIOError.ErrorCode);
  end else Result := E.ClassName + ': ' + E.Message;
end;



function ShiftBytes(Buffer: PansiChar; Count: Word): LongWord;
begin
	Result := 0;

	case Count of
  	1: Result := Byte(Buffer[0]);
	  2: begin
	   	Result := Byte(Buffer[0]) shl $08;
		  Result := Result or Byte(Buffer[1]);
    end;
  	3: begin
   		Result := Byte(Buffer[0]) shl $10;
		  Result := Result or (Byte(Buffer[1]) shl $08);
		  Result := Result or Byte(Buffer[2]);
    end;
   	4: begin
      Result := Byte(Buffer[0]) shl $18;
	   	Result := Result or (Byte(Buffer[1]) shl $10);
	   	Result := Result or (Byte(Buffer[2]) shl $08);
	  	Result := Result or Byte(Buffer[3]);
    end;
  end;
end;



function GetUncompressedSize(Buffer: PAnsiChar): LongWord;
var
  Flag: Word;
begin
	Result := 0;
  Flag := ShiftBytes(Buffer, 2);

	if ((Flag and $3EFF) = $10FB) then begin
		Flag := Flag and $8000;
		if (Flag > 0) then Flag := 1;
		Inc(Flag, 3);
		Result := ShiftBytes(Buffer + 2, Flag);
  end;
end;



procedure GetUncompressedData(InBuffer, OutBuffer: PAnsiChar);
var
	InBuffer2, OutBuffer2, TempBuf: PAnsiChar;
  Code, Code2, Code3, Code4: Byte;
  Count, Flags, i: LongWord;
begin
  InBuffer2 := inbuffer;
  OutBuffer2 := outbuffer;
  Flags := Byte(InBuffer2^);
  Inc(Inbuffer2);
  Flags := (Flags shl $08) + Byte(InBuffer2^);
	Inc(InBuffer2);

	if ((Flags and $8000) > 0) then begin
		if ((Flags and $100) > 0) then Inc(InBuffer2, 4);
		Inc(InBuffer2, 4);
  end
	else begin
		if ((Flags and $100) > 0) then Inc(InBuffer2, 3);
		Inc(InBuffer2, 3);
  end;

  while (true) do begin
		Code := Byte(InBuffer2^);
		Inc(InBuffer2);

		if ((Code and $80) = 0) then begin
			Code2 := Byte(InBuffer2^);
			Inc(InBuffer2);
			Count := Code and $03;

      if (Count > 0) then for i := 0 to Count - 1 do begin
				OutBuffer2^ := InBuffer2^;
				Inc(InBuffer2);
				Inc(OutBuffer2);
      end;

			TempBuf := (OutBuffer2 - 1) - (Code2 + (Code and $60) * 8);
			Count := (((Code and $1C) div 4) + 2);

			for i := 0 to Count do begin
				OutBuffer2^ := TempBuf^;
				Inc(OutBuffer2);
				Inc(TempBuf);
      end;
    end
		else if ((code and $40) = 0) then begin
			code2 := Byte(inbuffer2^);
			Inc(InBuffer2);
			code3 := Byte(inbuffer2^);
			Inc(InBuffer2);
			count := code2 shr 6;

			if (Count > 0) then for i := 0 to Count - 1 do begin
				outbuffer2^ := inbuffer2^;
				Inc(InBuffer2);
				Inc(OutBuffer2);
      end;

			tempbuf := (outbuffer2 - 1) - (((code2 and $3F) shl 8) + code3);
			count := (code and $3F) + 3;

			for i := 0 to Count do begin
				outbuffer2^ := tempbuf^;
				Inc(OutBuffer2);
				Inc(TempBuf);
      end;
    end
		else if ((code and $20) = 0) then begin
			code2 := Byte(InBuffer2^);
			Inc(InBuffer2);
			code3 := Byte(InBuffer2^);
			Inc(InBuffer2);
			code4 := Byte(InBuffer2^);
			Inc(InBuffer2);
			count := code and 3;

			if (Count > 0) then for i := 0 to Count - 1 do begin
				outbuffer2^ := inbuffer2^;
				Inc(InBuffer2);
				Inc(OutBuffer2);
      end;

			tempbuf := (outbuffer2 - 1) - ((((code and $10) shr 4) shl $10) + (code2 shl 8) + code3);
			count := (((code and $0C) shr 2) shl 8) + code4 + 4;

			for i := 0 to Count do begin
				outbuffer2^ := tempbuf^;
				Inc(OutBuffer2);
				Inc(TempBuf);
      end;
    end
		else begin
			count := ((code and $1F) * 4) + 4;

			if (count <= $70) then begin
				if (Count > 0) then for i := 0 to Count - 1 do begin
					outbuffer2^ := inbuffer2^;
				  Inc(InBuffer2);
				  Inc(OutBuffer2);
				end;
      end
			else begin
				count := code and 3;

				if (Count > 0) then for i := 0 to Count - 1 do begin
					outbuffer2^ := inbuffer2^;
		   		Inc(InBuffer2);
		   		Inc(OutBuffer2);
				end;

				Break;
      end;
    end;
  end;
end;



// Extract Big
procedure ExtractBig(FileName: WideString);
var
  InBuffer, OutBuffer: PAnsiChar;
  BigHeader: TBigHeader;
  BigEntry: TBigEntry;
  BigFile, OutFile: File;
  n, TempPos, TempSize: LongWord;
  PFileName: PAnsiChar;
begin
  if not (LineWritten) then begin
    WriteLn('Extracting worldbuilder stream files');
    WriteLn('This may take a while and your computer may not respond during the extraction due to a huge data amount. Please wait until the process finished!');
    LineWritten := true;
  end;

  AssignFile(BigFile, WorldbuilderBig);
  FileMode := fmOpenRead;
  Reset(BigFile, 1);
  BlockRead(BigFile, BigHeader, 16);
  BigHeader.FileCount := SwapDWord(BigHeader.FileCount);
  BigHeader.DataOffset := SwapDWord(BigHeader.DataOffset);

  if (BigHeader.FileCount > 0) then for n := 0 to BigHeader.FileCount - 1 do begin
    BlockRead(BigFile, BigEntry, 8);
    BigEntry.EntryOffset := SwapDWord(BigEntry.EntryOffset);
    BigEntry.EntrySize := SwapDWord(BigEntry.EntrySize);

    TempPos := FilePos(BigFile);
    GetMem(PFileName, BigHeader.DataOffset - LongWord(FilePos(BigFile)));
    BlockRead(BigFile, PFileName^, BigHeader.DataOffset - LongWord(FilePos(BigFile)));

    if (LowerCase(FileName) = LowerCase(WideString(PFileName))) then begin
      Seek(BigFile, BigEntry.EntryOffset);
      GetMem(InBuffer, BigEntry.EntrySize);
      BlockRead(BigFile, InBuffer^, BigEntry.EntrySize);
      TempSize := GetUncompressedSize(InBuffer);

      AssignFile(OutFile, ManifestPath + ExtractFileName(WideString(PFileName)));
      FileMode := fmOpenWrite;
      Rewrite(OutFile, 1);

      if (TempSize > BigEntry.EntrySize) then begin
        GetMem(OutBuffer, TempSize);
        GetUncompressedData(Inbuffer, OutBuffer);
        BlockWrite(OutFile, OutBuffer^, TempSize);
        FreeMem(OutBuffer);
      end
      else BlockWrite(OutFile, InBuffer^, BigEntry.EntrySize);

      FreeMem(InBuffer);
      FreeMem(PFileName);
      CloseFile(OutFile);
      Break;
    end;

    Seek(BigFile, TempPos + LongWord(Length(PFileName)) + 1);
    FreeMem(PFileName);
  end;

  CloseFile(BigFile);
end;



// Check Asset
function CheckAsset(Asset: TAssetReference): LongWord;
var
  j, k, InsertBinOffset, InsertImpOffset, InsertReloOffset, NodeCount, HashInt: LongWord;
  AssetExists, TypeHashExists, InstanceHashExists: Boolean;
  AssetList: array of TAssetReference;
  InsertString: PAnsiChar;
  XMLDoc: IXMLDocument;
  NodeAssetDeclaration: IXMLNode;
  StringHashesFile: TextFile;
  HashName: WideString;
begin
  Result := 0;
  InsertBinOffset := 4;
  InsertImpOffset := 4;
  InsertReloOffset := 4;
  AssetExists := false;

  // Check Static
  if (Length(StaticAssetList) > 0) then for j := 0 to High(StaticAssetList) do begin
    if ((Asset.InstanceID = StaticAssetList[j].InstanceID) and ((Asset.TypeID = StaticAssetList[j].TypeID) or ((Asset.TypeID = $b22f87c9) and ((StaticAssetList[j].TypeID = $e3181c04) or (StaticAssetList[j].TypeID = $f0f08712) or (StaticAssetList[j].TypeID = $c2b1a262))))) then begin
      Result := StaticAssetList[j].TypeID;
      AssetExists := true;
      Break;
    end;
  end;

  if not (AssetExists) then begin
    // Check Global
    if (Length(GlobalAssetList) > 0) then for j := 0 to High(GlobalAssetList) do begin
      if ((Asset.InstanceID = GlobalAssetList[j].InstanceID) and ((Asset.TypeID = GlobalAssetList[j].TypeID) or ((Asset.TypeID = $b22f87c9) and ((GlobalAssetList[j].TypeID = $e3181c04) or (GlobalAssetList[j].TypeID = $f0f08712) or (GlobalAssetList[j].TypeID = $c2b1a262))))) then begin
        Result := GlobalAssetList[j].TypeID;
        AssetExists := true;
        Break;
      end;
    end;

    if not (AssetExists) then begin
      // Check Audio
      if (Length(AudioAssetList) > 0) then for j := 0 to High(AudioAssetList) do begin
        if ((Asset.InstanceID = AudioAssetList[j].InstanceID) and ((Asset.TypeID = AudioAssetList[j].TypeID) or ((Asset.TypeID = $b22f87c9) and ((AudioAssetList[j].TypeID = $e3181c04) or (AudioAssetList[j].TypeID = $f0f08712) or (AudioAssetList[j].TypeID = $c2b1a262))))) then begin
          Result := AudioAssetList[j].TypeID;
          AssetExists := true;
          Break;
        end;
      end;

      if not (AssetExists) then begin
        // Check Worldbuilder
        if (Length(WorldbuilderAssetList) > 0) then for j := 0 to High(WorldbuilderAssetList) do begin
          if ((Asset.InstanceID = WorldbuilderAssetList[j].InstanceID) and ((Asset.TypeID = WorldbuilderAssetList[j].TypeID) or ((Asset.TypeID = $b22f87c9) and ((WorldbuilderAssetList[j].TypeID = $e3181c04) or (WorldbuilderAssetList[j].TypeID = $f0f08712) or (WorldbuilderAssetList[j].TypeID = $c2b1a262))))) then begin
            Result := WorldbuilderAssetList[j].TypeID;

            if (Length(InsertAssetList) > 0) then begin
              Asset.TypeID := WorldbuilderAssetList[j].TypeID;

              for k := 0 to High(InsertAssetList) do begin
                if ((Asset.TypeID = InsertAssetList[k].TypeID) and (Asset.InstanceID = InsertAssetList[k].InstanceID)) then begin
                  AssetExists := true;
                  Break;
                end;
              end;
            end;

            if not (AssetExists) then begin
              SetLength(AssetList, WorldbuilderAssetList[j].AssetReferenceCount);
              Seek(WorldbuilderFile, ManifestHeaderSize + WorldbuilderHeader.AssetCount * ManifestAssetSize + WorldbuilderAssetList[j].AssetReferenceOffset);
              BlockRead(WorldbuilderFile, AssetList[0], WorldbuilderAssetList[j].AssetReferenceCount * ManifestAssetReferenceSize);

              if (Length(AssetList) > 0) then for k := 0 to High(AssetList) do CheckAsset(AssetList[k]);

              // Insert Routine
              // Manifest
              SetLength(InsertAssetList, Length(InsertAssetList) + 1);
              InsertAssetList[High(InsertAssetList)] := WorldbuilderAssetList[j];
              InsertAssetList[High(InsertAssetList)].AssetReferenceOffset := InsertAssetReferenceOffset;
              InsertAssetList[High(InsertAssetList)].NameOffset := InsertNameOffset;
              InsertAssetList[High(InsertAssetList)].SourceFileNameOffset := InsertSourceFileNameOffset;

              SetLength(InsertAssetReferenceBuffer, LongWord(Length(InsertAssetReferenceBuffer)) + WorldbuilderAssetList[j].AssetReferenceCount);
              if (WorldbuilderAssetList[j].AssetReferenceCount > 0) then for k := 0 to WorldbuilderAssetList[j].AssetReferenceCount - 1 do InsertAssetReferenceBuffer[LongWord(High(InsertAssetReferenceBuffer)) - (WorldbuilderAssetList[j].AssetReferenceCount - k - 1)] := AssetList[k];
              Inc(InsertAssetReferenceOffset, WorldbuilderAssetList[j].AssetReferenceCount * ManifestAssetReferenceSize);

              Seek(WorldbuilderFile, ManifestHeaderSize + (WorldbuilderHeader.AssetCount * ManifestAssetSize) + WorldbuilderHeader.AssetReferenceBufferSize + WorldbuilderHeader.ReferencedManifestNameBufferSize + WorldbuilderAssetList[j].NameOffset);
              GetMem(InsertString, WorldbuilderHeader.AssetNameBufferSize - WorldbuilderAssetList[j].NameOffset);
              BlockRead(WorldbuilderFile, InsertString^, WorldbuilderHeader.AssetNameBufferSize - WorldbuilderAssetList[j].NameOffset);
              Inc(InsertNameOffset, Length(InsertString) + 1);
              InsertNameBuffer := InsertNameBuffer + InsertString + #00;
              FreeMem(InsertString);

              Seek(WorldbuilderFile, ManifestHeaderSize + (WorldbuilderHeader.AssetCount * ManifestAssetSize) + WorldbuilderHeader.AssetReferenceBufferSize + WorldbuilderHeader.ReferencedManifestNameBufferSize + WorldbuilderHeader.AssetNameBufferSize + WorldbuilderAssetList[j].SourceFileNameOffset);
              GetMem(InsertString, WorldbuilderHeader.SourceFileNameBufferSize - WorldbuilderAssetList[j].SourceFileNameOffset);
              BlockRead(WorldbuilderFile, InsertString^, WorldbuilderHeader.SourceFileNameBufferSize - WorldbuilderAssetList[j].SourceFileNameOffset);
              Inc(InsertSourceFileNameOffset, Length(InsertString) + 1);
              InsertSourceFileNameBuffer := InsertSourceFileNameBuffer + InsertString + #00;
              FreeMem(InsertString);

              // Bin
              SetLength(InsertBin, LongWord(Length(InsertBin)) + WorldbuilderAssetList[j].InstanceDataSize);
              Seek(WorldbuilderBinFile, InsertBinOffset);
              BlockRead(WorldbuilderBinFile, InsertBin[LongWord(Length(InsertBin)) - WorldbuilderAssetList[j].InstanceDataSize + 1], WorldbuilderAssetList[j].InstanceDataSize);

              // Imp
              SetLength(InsertImp, LongWord(Length(InsertImp)) + WorldbuilderAssetList[j].ImportsDataSize);
              Seek(WorldbuilderImpFile, InsertImpOffset);
              BlockRead(WorldbuilderImpFile, InsertImp[LongWord(Length(InsertImp)) - WorldbuilderAssetList[j].ImportsDataSize + 1], WorldbuilderAssetList[j].ImportsDataSize);

              // Relo
              SetLength(InsertRelo, LongWord(Length(InsertRelo)) + WorldbuilderAssetList[j].RelocationDataSize);
              Seek(WorldbuilderReloFile, InsertReloOffset);
              BlockRead(WorldbuilderReloFile, InsertRelo[LongWord(Length(InsertRelo)) - WorldbuilderAssetList[j].RelocationDataSize + 1], WorldbuilderAssetList[j].RelocationDataSize);
            end;

            AssetExists := true;
            Break;
          end;

          Inc(InsertBinOffset, WorldbuilderAssetList[j].InstanceDataSize);
          Inc(InsertImpOffset, WorldbuilderAssetList[j].ImportsDataSize);
          Inc(InsertReloOffset, WorldbuilderAssetList[j].RelocationDataSize);
        end;

        if not (AssetExists) then begin
          // Check Output
          if (Length(DanglingAssets) > 0) then for j := 0 to High(DanglingAssets) do begin
            if ((Asset.TypeID = DanglingAssets[j].TypeID) and (Asset.InstanceID = DanglingAssets[j].InstanceID)) then begin
              AssetExists := true;
              Break;
            end;
          end;

          if not (AssetExists) then begin
            SetLength(DanglingAssets, Length(DanglingAssets) + 1);
            DanglingAssets[High(DanglingAssets)].TypeID := Asset.TypeID;
            DanglingAssets[High(DanglingAssets)].InstanceID := Asset.InstanceID;
            TypeHashExists := false;
            InstanceHashExists := false;
            NodeCount := 0;

            XMLDoc := TXMLDocument.Create(nil);
            XMLDoc.LoadFromFile(StringHashesXml);
            NodeAssetDeclaration := XMLDoc.ChildNodes.FindNode('AssetDeclaration');

            if (not (NodeAssetDeclaration = nil) and (NodeAssetDeclaration.ChildNodes.Count > 0)) then for j := 0 to NodeAssetDeclaration.ChildNodes.Count - 1 do begin
              if (LowerCase(NodeAssetDeclaration.ChildNodes[j].Attributes['id']) = 'stringhashbin_typeid') then begin
                if (NodeAssetDeclaration.ChildNodes[j].ChildNodes.Count > 0) then for k := 0 to NodeAssetDeclaration.ChildNodes[j].ChildNodes.Count - 1 do begin
                  if (LongWord(StrToInt64(NodeAssetDeclaration.ChildNodes[j].ChildNodes[k].Attributes['Hash'])) = Asset.TypeID) then begin
                    DanglingAssets[High(DanglingAssets)].TypeName := NodeAssetDeclaration.ChildNodes[j].ChildNodes[k].Attributes['Text'];
                    TypeHashExists := true;
                    Break;
                  end;
                end;

                Inc(NodeCount);
              end
              else if (LowerCase(NodeAssetDeclaration.ChildNodes[j].Attributes['id']) = 'stringhashbin_instanceid') then begin
                if (NodeAssetDeclaration.ChildNodes[j].ChildNodes.Count > 0) then for k := 0 to NodeAssetDeclaration.ChildNodes[j].ChildNodes.Count - 1 do begin
                  if (LongWord(StrToInt64(NodeAssetDeclaration.ChildNodes[j].ChildNodes[k].Attributes['Hash'])) = Asset.InstanceID) then begin
                    DanglingAssets[High(DanglingAssets)].InstanceName := NodeAssetDeclaration.ChildNodes[j].ChildNodes[k].Attributes['Text'];
                    InstanceHashExists := true;
                    Break;
                  end;
                end;

                Inc(NodeCount);
              end;

              if (NodeCount = 2) then Break;
            end;

            if not (TypeHashExists) then begin
              AssignFile(StringHashesFile, IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'ModAssetResolver_TypeHashes.txt');
              FileMode := fmOpenRead;
              Reset(StringHashesFile);

              while not Eof(StringHashesFile) do begin
                ReadLn(StringHashesFile, HashInt);

                if (HashInt = Asset.TypeID) then begin
                  ReadLn(StringHashesFile, HashName);
                  DanglingAssets[High(DanglingAssets)].TypeName := HashName;
                  TypeHashExists := true;
                  Break;
                end else ReadLn(StringHashesFile);
              end;

              CloseFile(StringHashesFile)
            end;

            if not (InstanceHashExists) then begin
              AssignFile(StringHashesFile, IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'ModAssetResolver_InstanceHashes.txt');
              FileMode := fmOpenRead;
              Reset(StringHashesFile);

              while not Eof(StringHashesFile) do begin
                ReadLn(StringHashesFile, HashInt);

                if (HashInt = Asset.InstanceID) then begin
                  ReadLn(StringHashesFile, HashName);
                  DanglingAssets[High(DanglingAssets)].InstanceName := HashName;
                  InstanceHashExists := true;
                  Break;
                end else ReadLn(StringHashesFile);
              end;

              CloseFile(StringHashesFile)
            end;

            if not (TypeHashExists) then DanglingAssets[High(DanglingAssets)].TypeName := IntToStr(Asset.TypeID);
            if not (InstanceHashExists) then DanglingAssets[High(DanglingAssets)].InstanceName := IntToStr(Asset.InstanceID);
          end;
        end;
      end;
    end;
  end;
end;



// Main
procedure Main;
var
  ModManifest, AudioManifest, GlobalManifest, StaticManifest, WorldbuilderManifest, WorldbuilderBin, WorldbuilderImp, WorldbuilderRelo: WideString;
  ModFile, AudioFile, GlobalFile, StaticFile: File;
  ModHeader, AudioHeader, GlobalHeader, StaticHeader: TManifestHeader;
  ModAssetReferenceBuffer: array of TAssetReference;
  i, j, m, AssetTypeID: LongWord;
  ModData, ModReferencedManifestBuffer, ModNameBuffer, ModSourceFileNameBuffer: AnsiString;
  AssetExists, FileNotFound: Boolean;
  Reg: TRegistry;
begin
  WriteLn('Asset Resolver by Bibber');

  if (ParamCount < 1) then WriteLn('Usage: AssetResolver.exe MapOrModManifestFile')
  else begin
    Reg := TRegistry.Create;
    Reg.RootKey := HKEY_LOCAL_MACHINE;

    if (Reg.OpenKeyReadOnly('SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{F6A3F605-7B10-4939-8D3D-4594332C1649}')) then begin
      ManifestPath := IncludeTrailingPathDelimiter(Reg.ReadString('InstallLocation'));
      Reg.CloseKey;
    end
    else if (Reg.OpenKeyReadOnly('SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{F6A3F605-7B10-4939-8D3D-4594332C1649}')) then begin
      ManifestPath := IncludeTrailingPathDelimiter(Reg.ReadString('InstallLocation'));
      Reg.CloseKey;
    end
    else ManifestPath := '..\';

    if (Reg.OpenKeyReadOnly('SOFTWARE\Electronic Arts\Electronic Arts\Red Alert 3')) then begin
      WorldbuilderBig := IncludeTrailingPathDelimiter(Reg.ReadString('Install Dir'));
      Reg.CloseKey;
    end
    else if (Reg.OpenKeyReadOnly('SOFTWARE\Wow6432Node\Electronic Arts\Electronic Arts\Red Alert 3')) then begin
      WorldbuilderBig := IncludeTrailingPathDelimiter(Reg.ReadString('Install Dir'));
      Reg.CloseKey;
    end
    else WorldbuilderBig := '';

    FreeAndNil(Reg);
    ModManifest := ParamStr(1);
    ManifestPath := ManifestPath + 'BuiltMods\';
    WorldbuilderBig := WorldbuilderBig + 'Data\WBData_12.big';
    StringHashesXml := ManifestPath + 'StringHashes.xml';
    AudioManifest := ManifestPath + 'Audio.manifest';
    GlobalManifest := ManifestPath + 'Global.manifest';
    StaticManifest := ManifestPath + 'Static.manifest';
    WorldbuilderManifest := ManifestPath + 'Worldbuilder.manifest';
    WorldbuilderBin := ChangeFileExt(WorldbuilderManifest, '.bin');
    WorldbuilderImp := ChangeFileExt(WorldbuilderManifest, '.imp');
    WorldbuilderRelo := ChangeFileExt(WorldbuilderManifest, '.relo');
    FileNotFound := false;
    LineWritten := false;

    if not (FileExists(WorldbuilderManifest)) then begin
      if (FileExists(WorldbuilderBig)) then ExtractBig('Data\' + ExtractFileName(WorldbuilderManifest))
      else FileNotFound := true;
    end;

    if (not (FileExists(WorldbuilderImp)) and not (FileNotFound)) then begin
      if (FileExists(WorldbuilderBig)) then ExtractBig('Data\' + ExtractFileName(WorldbuilderImp))
      else FileNotFound := true;
    end;

    if (not (FileExists(WorldbuilderRelo)) and not (FileNotFound)) then begin
      if (FileExists(WorldbuilderBig)) then ExtractBig('Data\' + ExtractFileName(WorldbuilderRelo))
      else FileNotFound := true;
    end;

    if ((not FileExists(WorldbuilderBin)) and not (FileNotFound)) then begin
      if (FileExists(WorldbuilderBig)) then ExtractBig('Data\' + ExtractFileName(WorldbuilderBin))
      else FileNotFound := true;
    end;

    if (FileNotFound) then WriteLn('Error: "' + WorldbuilderBig + '" not found')
    else if not (FileExists(AudioManifest)) then WriteLn('Error: "' + AudioManifest + '" not found')
    else if not (FileExists(GlobalManifest)) then WriteLn('Error: "' + GlobalManifest + '" not found')
    else if not (FileExists(StaticManifest)) then WriteLn('Error: "' + StaticManifest + '" not found')
    else if not (FileExists(ModManifest)) then WriteLn('Error: "' + ModManifest + '" not found')
    else if not (FileExists(StringHashesXml)) then WriteLn('Error: "' + StringHashesXml + '" not found')
    else begin
      // Initialization
      SetLength(InsertAssetList, 0);
      SetLength(InsertAssetReferenceBuffer, 0);
      InsertAssetReferenceOffset := 0;
      InsertNameOffset := 0;
      InsertSourceFileNameOffset := 0;
      InsertNameBuffer := '';
      InsertSourceFileNameBuffer := '';
      SetLength(DanglingAssets, 0);
      SetLength(InsertBin, 0);
      SetLength(InsertImp, 0);
      SetLength(InsertRelo, 0);

      // Load String Hashes
      WriteLn('Loading data');

      // Load Mod
      AssignFile(ModFile, ModManifest);
      FileMode := fmOpenReadWrite;
      Reset(ModFile, 1);
      BlockRead(ModFile, ModHeader, ManifestHeaderSize);

      SetLength(ModAssetList, ModHeader.AssetCount);
      BlockRead(ModFile, ModAssetList[0], ManifestAssetSize * ModHeader.AssetCount);

      SetLength(ModAssetReferenceBuffer, ModHeader.AssetReferenceBufferSize div ManifestAssetReferenceSize);
      BlockRead(ModFile, ModAssetReferenceBuffer[0], ModHeader.AssetReferenceBufferSize);

      // Load Audio
      AssignFile(AudioFile, AudioManifest);
      FileMode := fmOpenRead;
      Reset(AudioFile, 1);
      BlockRead(AudioFile, AudioHeader, ManifestHeaderSize);

      SetLength(AudioAssetList, AudioHeader.AssetCount);
      BlockRead(AudioFile, AudioAssetList[0], ManifestAssetSize * AudioHeader.AssetCount);

      // Load Global
      AssignFile(GlobalFile, GlobalManifest);
      FileMode := fmOpenRead;
      Reset(GlobalFile, 1);
      BlockRead(GlobalFile, GlobalHeader, ManifestHeaderSize);

      SetLength(GlobalAssetList, GlobalHeader.AssetCount);
      BlockRead(GlobalFile, GlobalAssetList[0], ManifestAssetSize * GlobalHeader.AssetCount);

      // Load Static
      AssignFile(StaticFile, StaticManifest);
      FileMode := fmOpenRead;
      Reset(StaticFile, 1);
      BlockRead(StaticFile, StaticHeader, ManifestHeaderSize);

      SetLength(StaticAssetList, StaticHeader.AssetCount);
      BlockRead(StaticFile, StaticAssetList[0], ManifestAssetSize * StaticHeader.AssetCount);

      // Load Worldbuilder
      AssignFile(WorldbuilderFile, WorldbuilderManifest);
      FileMode := fmOpenRead;
      Reset(WorldbuilderFile, 1);
      BlockRead(WorldbuilderFile, WorldbuilderHeader, ManifestHeaderSize);

      SetLength(WorldbuilderAssetList, WorldbuilderHeader.AssetCount);
      BlockRead(WorldbuilderFile, WorldbuilderAssetList[0], ManifestAssetSize * WorldbuilderHeader.AssetCount);

      AssignFile(WorldbuilderBinFile, WorldbuilderBin);
      FileMode := fmOpenRead;
      Reset(WorldbuilderBinFile, 1);

      AssignFile(WorldbuilderImpFile, WorldbuilderImp);
      FileMode := fmOpenRead;
      Reset(WorldbuilderImpFile, 1);

      AssignFile(WorldbuilderReloFile, WorldbuilderRelo);
      FileMode := fmOpenRead;
      Reset(WorldbuilderReloFile, 1);

      // Check Assets
      WriteLn('Checking data');

      if (Length(ModAssetReferenceBuffer) > 0) then for i := 0 to High(ModAssetReferenceBuffer) do begin
        // Check Mod
        AssetExists := false;

        if (Length(ModAssetList) > 0) then for m := 0 to High(ModAssetList) do begin
          if ((ModAssetReferenceBuffer[i].InstanceID = ModAssetList[m].InstanceID) and ((ModAssetReferenceBuffer[i].TypeID = ModAssetList[m].TypeID) or ((ModAssetReferenceBuffer[i].TypeID = $b22f87c9) and ((ModAssetList[m].TypeID = $e3181c04) or (ModAssetList[m].TypeID = $f0f08712) or (ModAssetList[m].TypeID = $c2b1a262))))) then begin
            AssetExists := true;
            Break;
          end;
        end;

        if not (AssetExists) then begin
          AssetTypeID := CheckAsset(ModAssetReferenceBuffer[i]);
          if (AssetTypeID > 0) then ModAssetReferenceBuffer[i].TypeID := AssetTypeID;
        end;
      end;

      CloseFile(AudioFile);
      CloseFile(GlobalFile);
      CloseFile(StaticFile);
      CloseFile(WorldbuilderFile);
      CloseFile(WorldbuilderBinFile);
      CloseFile(WorldbuilderImpFile);
      CloseFile(WorldbuilderReloFile);

      // Save Manifest
      WriteLn('Saving data');

      SetLength(ModReferencedManifestBuffer, ModHeader.ReferencedManifestNameBufferSize);
      BlockRead(ModFile, ModReferencedManifestBuffer[1], ModHeader.ReferencedManifestNameBufferSize);

      SetLength(ModNameBuffer, ModHeader.AssetNameBufferSize);
      BlockRead(ModFile, ModNameBuffer[1], ModHeader.AssetNameBufferSize);

      SetLength(ModSourceFileNameBuffer, ModHeader.SourceFileNameBufferSize);
      BlockRead(ModFile, ModSourceFileNameBuffer[1], ModHeader.SourceFileNameBufferSize);

      if (Length(ModAssetList) > 0) then for i := 0 to High(ModAssetList) do begin
        Inc(ModAssetList[i].AssetReferenceOffset, Length(InsertAssetReferenceBuffer) * 8);
        Inc(ModAssetList[i].NameOffset, Length(InsertNameBuffer));
        Inc(ModAssetList[i].SourceFileNameOffset, Length(InsertSourceFileNameBuffer));
      end;

      if (Length(InsertAssetList) > 0) then for i := 0 to High(InsertAssetList) do begin
        if (InsertAssetList[i].InstanceDataSize > ModHeader.MaxInstanceChunkSize) then ModHeader.MaxInstanceChunkSize := InsertAssetList[i].InstanceDataSize;
        if (InsertAssetList[i].ImportsDataSize > ModHeader.MaxImportsChunkSize) then ModHeader.MaxImportsChunkSize := InsertAssetList[i].ImportsDataSize;
        if (InsertAssetList[i].RelocationDataSize > ModHeader.MaxRelocationChunkSize) then ModHeader.MaxRelocationChunkSize := InsertAssetList[i].RelocationDataSize;
      end;

      Inc(ModHeader.AssetCount, Length(InsertAssetList));
      Inc(ModHeader.TotalInstanceDataSize, Length(InsertBin));
      Inc(ModHeader.AssetReferenceBufferSize, Length(InsertAssetReferenceBuffer) * ManifestAssetReferenceSize);
      Inc(ModHeader.AssetNameBufferSize, Length(InsertNameBuffer));
      Inc(ModHeader.SourceFileNameBufferSize, Length(InsertSourceFileNameBuffer));

      Rewrite(ModFile, 1);
      BlockWrite(ModFile, ModHeader, ManifestHeaderSize);

      BlockWrite(ModFile, InsertAssetList[0], Length(InsertAssetList) * ManifestAssetSize);
      BlockWrite(ModFile, ModAssetList[0], Length(ModAssetList) * ManifestAssetSize);

      BlockWrite(ModFile, InsertAssetReferenceBuffer[0], Length(InsertAssetReferenceBuffer) * ManifestAssetReferenceSize);
      BlockWrite(ModFile, ModAssetReferenceBuffer[0], Length(ModAssetReferenceBuffer) * ManifestAssetReferenceSize);

      BlockWrite(ModFile, ModReferencedManifestBuffer[1], Length(ModReferencedManifestBuffer));

      BlockWrite(ModFile, InsertNameBuffer[1], Length(InsertNameBuffer));
      BlockWrite(ModFile, ModNameBuffer[1], Length(ModNameBuffer));

      BlockWrite(ModFile, InsertSourceFileNameBuffer[1], Length(InsertSourceFileNameBuffer));
      BlockWrite(ModFile, ModSourceFileNameBuffer[1], Length(ModSourceFileNameBuffer));

      CloseFile(ModFile);

      // Save Bin
      AssignFile(ModFile, ChangeFileExt(ModManifest, '.bin'));
      FileMode := fmOpenReadWrite;
      Reset(ModFile, 1);

      SetLength(ModData, FileSize(ModFile) - 4);
      Seek(ModFile, 4);
      BlockRead(ModFile, ModData[1], Length(ModData));
      Seek(ModFile, 4);
      BlockWrite(ModFile, InsertBin[1], Length(InsertBin));
      BlockWrite(ModFile, ModData[1], Length(ModData));
      CloseFile(ModFile);

      // Save Imp
      AssignFile(ModFile, ChangeFileExt(ModManifest, '.imp'));
      FileMode := fmOpenReadWrite;
      Reset(ModFile, 1);

      SetLength(ModData, FileSize(ModFile) - 4);
      Seek(ModFile, 4);
      BlockRead(ModFile, ModData[1], Length(ModData));
      Seek(ModFile, 4);
      BlockWrite(ModFile, InsertImp[1], Length(InsertImp));
      BlockWrite(ModFile, ModData[1], Length(ModData));
      CloseFile(ModFile);

      // Save Relo
      AssignFile(ModFile, ChangeFileExt(ModManifest, '.relo'));
      FileMode := fmOpenReadWrite;
      Reset(ModFile, 1);

      SetLength(ModData, FileSize(ModFile) - 4);
      Seek(ModFile, 4);
      BlockRead(ModFile, ModData[1], Length(ModData));
      Seek(ModFile, 4);
      BlockWrite(ModFile, InsertRelo[1], Length(InsertRelo));
      BlockWrite(ModFile, ModData[1], Length(ModData));
      CloseFile(ModFile);

      WriteLn('Assets prepended: ' + IntToStr(Length(InsertAssetList)));
      WriteLn('Dangling assets remaining: ' + IntToStr(Length(DanglingAssets)));
      if (Length(DanglingAssets) > 0) then for i := 0 to High(DanglingAssets) do WriteLn(IntToHex(DanglingAssets[i].TypeID, 8) + ':' + IntToHex(DanglingAssets[i].InstanceID, 8) + ' ' + DanglingAssets[i].TypeName + ':' + DanglingAssets[i].InstanceName);
    end;
  end;
end;



// Program
begin
  CoInitialize(nil);

  try
    Main;
  except
    on E: Exception do WriteLn('Error: ' + GetError(E));
  end;

  CoUninitialize;
end.

//========================================================//
//========================================================//
//========================================================//
