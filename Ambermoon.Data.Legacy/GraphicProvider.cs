﻿using Ambermoon.Data.Enumerations;
using Ambermoon.Data.Legacy.Serialization;
using Ambermoon.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambermoon.Data.Legacy
{
    public class GraphicProvider : IGraphicProvider
    {
        struct GraphicFile
        {
            public string File;
            public int[] SubFiles; // null means all
            public int FileIndexOffset;
            public bool Optional;

            public GraphicFile(string file, int fileIndexOffset = 0)
            {
                File = file;
                SubFiles = null;
                FileIndexOffset = fileIndexOffset;
                Optional = false;
            }

            public GraphicFile(string file, int fileIndexOffset, int subFile, bool optional = false)
            {
                File = file;
                SubFiles = new int[1] { subFile };
                FileIndexOffset = fileIndexOffset;
                Optional = optional;
            }

            public GraphicFile(string file, int fileIndexOffset, params int[] subFiles)
            {
                File = file;
                SubFiles = subFiles;
                FileIndexOffset = fileIndexOffset;
                Optional = false;
            }
        };

        readonly GameData gameData;
        public Dictionary<int, Graphic> Palettes { get; }
        public Dictionary<int, int> NPCGraphicOffsets { get; } = new Dictionary<int, int>();

        // Note: Found this at 0x12BE in data1 hunk of AM2_CPU (1.05 german)
        // These are 3 color index mappings for:
        // - 2D non-world maps
        // - 2D world maps
        // - 3D maps
        // Each tile or wall provides a color index in the range 0..15.
        // This index is used inside the mapping to get the associated palette index.
        static readonly byte[] ColorIndexMapping = new byte[48]
        {
            0x00, 0x1F, 0x1E, 0x1D, 0x1C, 0x1B, 0x1A, 0x12, 0x13, 0x14, 0x11, 0x10, 0x09, 0x0A, 0x18, 0x17,
            0x00, 0x01, 0x1F, 0x12, 0x1C, 0x14, 0x15, 0x06, 0x08, 0x0A, 0x04, 0x02, 0x0E, 0x0C, 0x13, 0x10,
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        public byte PaletteIndexFromColorIndex(Map map, byte colorIndex)
        {
            int offset = map.Type == MapType.Map3D ? 32 : map.IsWorldMap ? 16 : 0;
            return ColorIndexMapping[offset + colorIndex % 16];
        }

        public byte DefaultTextPaletteIndex => PrimaryUIPaletteIndex;
        public byte PrimaryUIPaletteIndex { get; }
        public byte SecondaryUIPaletteIndex { get; }
        public byte AutomapPaletteIndex { get; }
        public byte FirstIntroPaletteIndex { get; }
        public byte FirstOutroPaletteIndex { get; }
        public byte FirstFantasyIntroPaletteIndex { get; }

        public GraphicProvider(GameData gameData, ExecutableData.ExecutableData executableData, List<Graphic> additionalPalettes)
        {
            this.gameData = gameData;
            var graphicReader = new GraphicReader();
            Palettes = gameData.Files[paletteFile].Files.ToDictionary(f => f.Key, f => ReadPalette(graphicReader, f.Value));
            int i;

            PrimaryUIPaletteIndex = (byte)(1 + Palettes.Count);
            AutomapPaletteIndex = (byte)(PrimaryUIPaletteIndex + 1);
            SecondaryUIPaletteIndex = (byte)(PrimaryUIPaletteIndex + 2);
            FirstIntroPaletteIndex = (byte)(PrimaryUIPaletteIndex + 4);
            FirstOutroPaletteIndex = (byte)(FirstIntroPaletteIndex + 9);
            FirstFantasyIntroPaletteIndex = (byte)(FirstOutroPaletteIndex + 6);

            // Add builtin palettes
            for (i = 0; i < 3; ++i)
                Palettes.Add(PrimaryUIPaletteIndex + i, executableData.BuiltinPalettes[i]);

            // And another palette for some UI graphics.
            // The portraits have a blue gradient as background. It is also 32x34 pixels in size and the gradient
            // is in y-direction. All colors have R=0x00 and G=0x11. The blue component is increased by 0x11
            // every 2 pixels starting at y=4 (first 4 pixel rows have B=0x00, next 2 have B=0x11, etc).
            // Last 2 rows have B=0xff.
            Palettes.Add(PrimaryUIPaletteIndex + 3, new Graphic
            {
                Width = 32,
                Height = 1,
                IndexedGraphic = false,
                Data = new byte[]
                {
                    // The first colors are used for spells which use a materialize animation (earth and wind spells, waterfall, etc).
                    // The animation uses black, dark red, light purple, dark purple, dark beige, light beige in that order.
                    // We start with these color at offset 1. We leave the first color as fully transparent.
                    // Index 7 is unused.
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00, 0xff, 0x33, 0x11, 0x00, 0xff, 0x88, 0x77, 0xaa, 0xff,
                    0x66, 0x55, 0x88, 0xff, 0x99, 0x88, 0x77, 0xff, 0xbb, 0xbb, 0x99, 0xff, 0x00, 0x00, 0x00, 0x00,
                    // 16 colors for the blue background gradient of portraits
                    0x00, 0x11, 0x00, 0xff, 0x00, 0x11, 0x11, 0xff, 0x00, 0x11, 0x22, 0xff, 0x00, 0x11, 0x33, 0xff,
                    0x00, 0x11, 0x44, 0xff, 0x00, 0x11, 0x55, 0xff, 0x00, 0x11, 0x66, 0xff, 0x00, 0x11, 0x77, 0xff,
                    0x00, 0x11, 0x88, 0xff, 0x00, 0x11, 0x99, 0xff, 0x00, 0x11, 0xaa, 0xff, 0x00, 0x11, 0xbb, 0xff,
                    0x00, 0x11, 0xcc, 0xff, 0x00, 0x11, 0xdd, 0xff, 0x00, 0x11, 0xee, 0xff, 0x00, 0x11, 0xff, 0xff,
                    // some UI colors (TODO: character with condition?)
                    0x00, 0x00, 0x00, 0xff, 0x00, 0x00, 0x00, 0x00, 0x66, 0x66, 0x55, 0xff, 0x44, 0x44, 0x33, 0xff,
                    0x22, 0x22, 0x22, 0xff, 0x88, 0x88, 0x77, 0xff, 0xaa, 0xaa, 0x99, 0xff, 0xcc, 0xcc, 0xbb, 0xff
                }
            });

            const int additionalPaletteCount = 9 + 6 + 2; // Intro, Outro, Fantasy Intro
            i = 0;
            for (; i < Math.Min(additionalPaletteCount, additionalPalettes.Count); ++i)
            {
                Palettes.Add(FirstIntroPaletteIndex + i, additionalPalettes[i]);
            }
            for (; i < additionalPaletteCount; ++i)
            {
                Palettes.Add(FirstIntroPaletteIndex + i, new Graphic
                {
                    Width = 32,
                    Height = 1,
                    IndexedGraphic = false,
                    Data = new byte[32 * 4]
                });
            }

            foreach (var type in Enum.GetValues<GraphicType>())
            {
                if (type == GraphicType.Cursor)
                {
                    var cursorGraphics = graphics[GraphicType.Cursor] = new List<Graphic>();

                    foreach (var cursor in executableData.Cursors.Entries)
                        cursorGraphics.Add(cursor.Graphic);
                }
                else if (type == GraphicType.UIElements)
                {
                    graphics[type] = UIElementProvider.Create();
                    graphics[type].AddRange(executableData.UIGraphics.Entries.Values);
                    graphics[type].AddRange(executableData.Buttons.Entries.Values);
                }
                else if (type == GraphicType.TravelGfx)
                {
                    graphics[type] = gameData.TravelGraphics;
                }
                else if (type == GraphicType.Transports)
                {
                    var reader = gameData.Files["Stationary"].Files[1];
                    reader.Position = 0;
                    graphics[type] = gameData.StationaryImageInfos.Select(info =>
                    {
                        var graphic = new Graphic();
                        graphicReader.ReadGraphic(graphic, reader, info.Value);
                        return graphic;
                    }).ToList();
                }
                else if (type == GraphicType.NPC)
                {
                    var npcGraphics = new List<Graphic>(35);
                    var graphicInfo = new GraphicInfo
                    {
                        Width = 16,
                        Height = 32,
                        GraphicFormat = GraphicFormat.Palette5Bit,
                        Alpha = true,
                        PaletteOffset = 0
                    };
                    var graphic = new Graphic();
                    foreach (var file in gameData.Files["NPC_gfx.amb"].Files)
                    {
                        NPCGraphicOffsets.Add(file.Key, npcGraphics.Count);
                        var reader = file.Value;
                        reader.Position = 0;

                        while (reader.Position < reader.Size)
                        {
                            int numFrames = reader.ReadByte();

                            if (numFrames == 0)
                                break;

                            reader.AlignToWord();
                            var compoundGraphic = new Graphic(16 * numFrames, 32, 0);

                            for (i = 0; i < numFrames; ++i)
                            {
                                graphicReader.ReadGraphic(graphic, reader, graphicInfo);
                                compoundGraphic.AddOverlay((uint)i * 16, 0, graphic, false);
                            }

                            npcGraphics.Add(compoundGraphic);
                        }
                    }

                    graphics[type] = npcGraphics;
                }
                else if (type == GraphicType.CombatGraphics)
                {
                    var combatGraphics = new List<Graphic>(42);
                    var graphicInfo = new GraphicInfo
                    {
                        GraphicFormat = GraphicFormat.Palette5Bit,
                        Alpha = true,
                        PaletteOffset = 0
                    };
                    var reader = gameData.Files["Combat_graphics"].Files[1];
                    reader.Position = 0;

                    foreach (var combatGraphic in CombatGraphics.Info)
                    {
                        var info = combatGraphic.Value;

                        if (combatGraphic.Key == CombatGraphicIndex.BattleFieldIcons)
                        {
                            var battleFieldIcons = new List<Graphic>(36);
                            var iconGraphicInfo = new GraphicInfo
                            {
                                Width = 16,
                                Height = 14,
                                GraphicFormat = GraphicFormat.Palette5Bit,
                                Alpha = true,
                                PaletteOffset = 0
                            };

                            while (reader.Position < reader.Size)
                            {
                                var graphic = new Graphic();
                                graphicReader.ReadGraphic(graphic, reader, iconGraphicInfo);
                                battleFieldIcons.Add(graphic);
                            }

                            graphics[GraphicType.BattleFieldIcons] = battleFieldIcons;
                        }
                        else
                        {
                            var graphic = new Graphic();
                            var compoundGraphic = new Graphic((int)info.FrameCount * info.GraphicInfo.Width, info.GraphicInfo.Height, 0);

                            for (i = 0; i < info.FrameCount; ++i)
                            {
                                graphicReader.ReadGraphic(graphic, reader, info.GraphicInfo);
                                compoundGraphic.AddOverlay((uint)(i * info.GraphicInfo.Width), 0, graphic, false);
                            }

                            combatGraphics.Add(compoundGraphic);
                        }
                    }

                    graphics[type] = combatGraphics;
                }
                else if (type == GraphicType.BattleFieldIcons)
                {
                    // Do nothing. This is filled when processing GraphicType.CombatGraphics.
                }
                else if (type == GraphicType.RiddlemouthGraphics)
                {
                    var riddlemouthGraphics = new List<Graphic>(4 + 7);
                    var reader = gameData.Files["Riddlemouth_graphics"].Files[1];
                    reader.Position = 0;
                    // 4 eye frames
                    ReadAndAddGraphics(4, 48, 9);
                    // 7 mouth frames
                    ReadAndAddGraphics(7, 48, 15);
                    void ReadAndAddGraphics(int frames, int width, int height)
                    {
                        var graphicInfo = new GraphicInfo
                        {
                            Width = width,
                            Height = height,
                            GraphicFormat = GraphicFormat.Palette3Bit,
                            Alpha = false,
                            PaletteOffset = 24
                        };
                        var graphic = new Graphic();
                        var compoundGraphic = new Graphic(frames * width, height, 0);
                        for (int f = 0; f < frames; ++f)
                        {
                            graphicReader.ReadGraphic(graphic, reader, graphicInfo);
                            compoundGraphic.AddOverlay((uint)(f * width), 0, graphic, false);
                        }
                        riddlemouthGraphics.Add(compoundGraphic);
                    }
                    graphics[type] = riddlemouthGraphics;
                }
                else if (type == GraphicType.AutomapGraphics)
                {
                    var automapGraphics = new List<Graphic>(43);
                    var reader = gameData.Files["Automap_graphics"].Files[1];
                    reader.Position = 0x100; // TODO: maybe decode the bytes before that later

                    void ReadAndAddGraphics(int amount, int width, int height, GraphicFormat graphicFormat,
                        int frames = 1, bool alpha = false)
                    {
                        var graphicInfo = new GraphicInfo
                        {
                            Width = width,
                            Height = height,
                            GraphicFormat = graphicFormat,
                            Alpha = alpha,
                            PaletteOffset = 0
                        };
                        for (int i = 0; i < amount; ++i)
                        {
                            Graphic graphic = new Graphic();
                            if (frames == 1)
                            {
                                graphicReader.ReadGraphic(graphic, reader, graphicInfo);
                            }
                            else
                            {
                                var compoundGraphic = new Graphic(frames * width, height, 0);

                                for (int f = 0; f < frames; ++f)
                                {
                                    graphicReader.ReadGraphic(graphic, reader, graphicInfo);
                                    compoundGraphic.AddOverlay((uint)(f * width), 0, graphic, false);
                                }

                                graphic = compoundGraphic;
                            }
                            automapGraphics.Add(graphic);
                        }
                    }
                    // Map corners
                    ReadAndAddGraphics(4, 32, 32, GraphicFormat.Palette3Bit);
                    // Top map border
                    ReadAndAddGraphics(4, 16, 32, GraphicFormat.Palette3Bit);
                    // Right map border
                    ReadAndAddGraphics(2, 32, 32, GraphicFormat.Palette3Bit);
                    // Bottom map border
                    ReadAndAddGraphics(4, 16, 32, GraphicFormat.Palette3Bit);
                    // Left map border
                    ReadAndAddGraphics(2, 32, 32, GraphicFormat.Palette3Bit);
                    // 10 pin graphics
                    ReadAndAddGraphics(10, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Riddlemouth (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Teleport (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Spinner (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Trap (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Trapdoor (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Special (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Monster (4 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 4, true);
                    // Door closed (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Door open (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Merchant (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Inn (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Chest closed (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Exit (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Chest open (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Pile (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Person (1 frame)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 1, true);
                    // Goto point (8 frames)
                    ReadAndAddGraphics(1, 16, 16, GraphicFormat.Palette5Bit, 8, true);

                    graphics[type] = automapGraphics;
                }
                else
                {
                    LoadGraphics(type);
                }
            }
        }

        Graphic ReadPalette(GraphicReader graphicReader, IDataReader reader)
        {
            reader.Position = 0;
            var paletteGraphic = new Graphic();
            graphicReader.ReadGraphic(paletteGraphic, reader, paletteGraphicInfo);
            return paletteGraphic;
        }

        internal static Graphic ReadPalette(IDataReader reader)
        {
            var graphicReader = new GraphicReader();
            var paletteGraphic = new Graphic();
            graphicReader.ReadGraphic(paletteGraphic, reader, paletteGraphicInfo);
            return paletteGraphic;
        }

        static GraphicInfo paletteGraphicInfo = new GraphicInfo
        {
            Width = 32,
            Height = 1,
            GraphicFormat = GraphicFormat.XRGB16
        };
        static readonly string paletteFile = "Palettes.amb";
        static readonly Dictionary<GraphicType, GraphicFile[]> graphicFiles = new Dictionary<GraphicType, GraphicFile[]>();
        readonly Dictionary<GraphicType, List<Graphic>> graphics = new Dictionary<GraphicType, List<Graphic>>();

        static void AddGraphicFiles(GraphicType type, params GraphicFile[] files)
        {
            graphicFiles.Add(type, files);
        }

        static GraphicProvider()
        {
            AddGraphicFiles(GraphicType.Tileset1, new GraphicFile("1Icon_gfx.amb", 0, 1));
            AddGraphicFiles(GraphicType.Tileset2, new GraphicFile("3Icon_gfx.amb", 0, 2));
            AddGraphicFiles(GraphicType.Tileset3, new GraphicFile("2Icon_gfx.amb", 0, 3));
            AddGraphicFiles(GraphicType.Tileset4, new GraphicFile("2Icon_gfx.amb", 0, 4));
            AddGraphicFiles(GraphicType.Tileset5, new GraphicFile("2Icon_gfx.amb", 0, 5));
            AddGraphicFiles(GraphicType.Tileset6, new GraphicFile("2Icon_gfx.amb", 0, 6));
            AddGraphicFiles(GraphicType.Tileset7, new GraphicFile("2Icon_gfx.amb", 0, 7));
            AddGraphicFiles(GraphicType.Tileset8, new GraphicFile("3Icon_gfx.amb", 0, 8));
            AddGraphicFiles(GraphicType.Tileset9, new GraphicFile("3Icon_gfx.amb", 0, 9, true));
            AddGraphicFiles(GraphicType.Tileset10, new GraphicFile("2Icon_gfx.amb", 0, 10, true));
            AddGraphicFiles(GraphicType.Player, new GraphicFile("Party_gfx.amb"));
            AddGraphicFiles(GraphicType.Portrait, new GraphicFile("Portraits.amb"));
            AddGraphicFiles(GraphicType.Item, new GraphicFile("Object_icons"));
            AddGraphicFiles(GraphicType.Layout, new GraphicFile("Layouts.amb"));
            AddGraphicFiles(GraphicType.LabBackground, new GraphicFile("Lab_background.amb"));
            AddGraphicFiles(GraphicType.Pics80x80, new GraphicFile("Pics_80x80.amb"));
            AddGraphicFiles(GraphicType.EventPictures, new GraphicFile("Event_pix.amb"));
            AddGraphicFiles(GraphicType.CombatBackground, new GraphicFile("Combat_background.amb"));
        }

        public List<Graphic> GetGraphics(GraphicType type)
        {
            return graphics[type];
        }

        void LoadGraphics(GraphicType type)
        {
            if (!graphics.ContainsKey(type))
            {
                graphics.Add(type, new List<Graphic>());
                var reader = new GraphicReader();
                var info = GraphicInfoFromType(type);
                var graphicList = graphics[type];

                void LoadGraphic(IDataReader graphicDataReader, byte maskColor = 0)
                {
                    graphicDataReader.Position = 0;
                    int end = graphicDataReader.Size - info.DataSize;
                    while (graphicDataReader.Position <= end)
                    {
                        var graphic = new Graphic();
                        reader.ReadGraphic(graphic, graphicDataReader, info, maskColor);
                        graphicList.Add(graphic);
                    }
                }

                var allFiles = new SortedDictionary<int, IDataReader>();

                foreach (var graphicFile in graphicFiles[type])
                {
                    var containerFile = gameData.Files[graphicFile.File];

                    if (graphicFile.SubFiles == null)
                    {
                        foreach (var file in containerFile.Files)
                        {
                            allFiles[graphicFile.FileIndexOffset + file.Key] = file.Value;
                        }
                    }
                    else
                    {
                        foreach (var file in graphicFile.SubFiles)
                        {
                            if (containerFile.Files.TryGetValue(file, out var fileReader))
                                allFiles[graphicFile.FileIndexOffset + file] = fileReader;
                            else if (!graphicFile.Optional)
                                throw new KeyNotFoundException($"Sub file {file} of {graphicFile.File} was not found.");
                        }
                    }
                }

                foreach (var file in allFiles)
                {
                    LoadGraphic(file.Value, (byte)(type switch
                    {
                        GraphicType.Portrait => 25,
                        GraphicType.LabBackground => 9, // 9 is the sky color index
                        _ => 0
                    }));
                }
            }
        }

        GraphicInfo GraphicInfoFromType(GraphicType type)
        {
            var info = new GraphicInfo
            {
                Width = 16,
                Height = 16,
                GraphicFormat = GraphicFormat.Palette5Bit
            };

            switch (type)
            {
                case GraphicType.Tileset1:
                case GraphicType.Tileset2:
                case GraphicType.Tileset3:
                case GraphicType.Tileset4:
                case GraphicType.Tileset5:
                case GraphicType.Tileset6:
                case GraphicType.Tileset7:
                case GraphicType.Tileset8:
                case GraphicType.Tileset9:
                case GraphicType.Tileset10:
                    info.Alpha = true;
                    break;
                case GraphicType.Player:
                    info.Width = 16;
                    info.Height = 32;
                    info.Alpha = true;
                    break;
                case GraphicType.Portrait:
                    info.Width = 32;
                    info.Height = 34;
                    info.Alpha = false;
                    break;
                case GraphicType.Item:
                    info.Width = 16;
                    info.Height = 16;
                    info.Alpha = true;
                    break;
                case GraphicType.Layout:
                    info.Width = 320;
                    info.Height = 163;
                    info.GraphicFormat = GraphicFormat.Palette3Bit;
                    info.PaletteOffset = 24;
                    info.Alpha = true;
                    break;
                case GraphicType.LabBackground:
                    info.Width = 144;
                    info.Height = 20;
                    info.GraphicFormat = GraphicFormat.Palette4Bit;
                    info.PaletteOffset = 0;
                    info.Alpha = false;
                    break;
                case GraphicType.Pics80x80:
                    info.Width = 80;
                    info.Height = 80;
                    info.GraphicFormat = GraphicFormat.Palette5Bit;
                    info.PaletteOffset = 0;
                    info.Alpha = false;
                    break;
                case GraphicType.EventPictures:
                    info.Width = 320;
                    info.Height = 92;
                    info.GraphicFormat = GraphicFormat.Palette5Bit;
                    info.PaletteOffset = 0;
                    info.Alpha = false;
                    break;
                case GraphicType.CombatBackground:
                    info.Width = 320;
                    info.Height = 95;
                    info.GraphicFormat = GraphicFormat.Palette5Bit;
                    info.PaletteOffset = 0;
                    info.Alpha = false;
                    break;
            }

            return info;
        }

        public CombatBackgroundInfo Get2DCombatBackground(uint index) => CombatBackgrounds.Info2D[index];
        public CombatBackgroundInfo Get3DCombatBackground(uint index) => CombatBackgrounds.Info3D[index];
        public CombatGraphicInfo GetCombatGraphicInfo(CombatGraphicIndex index) => CombatGraphics.Info[index];
        public float GetMonsterRowImageScaleFactor(MonsterRow row) => row switch
        {
            MonsterRow.Farthest => 0.7f,
            MonsterRow.Far => 0.8f,
            MonsterRow.Near => 1.25f,
            _ => 1.0f,
        };
    }
}
