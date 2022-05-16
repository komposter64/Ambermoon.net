﻿/*
 * CustomOutro.cs - Remake outro sequence
 *
 * Copyright (C) 2021  Robert Schneckenhaus <robert.schneckenhaus@web.de>
 *
 * This file is part of Ambermoon.net.
 *
 * Ambermoon.net is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Ambermoon.net is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Ambermoon.net. If not, see <http://www.gnu.org/licenses/>.
 */

using Ambermoon.Data;
using Ambermoon.Data.Enumerations;
using Ambermoon.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TextColor = Ambermoon.Data.Enumerations.Color;

namespace Ambermoon.Render
{
    internal class CustomOutro
    {
        Credits credits = null;
        readonly Game game;
        readonly Layout layout;
        readonly IRenderView renderView;
        readonly Savegame savegame;
        readonly List<Popup> popups = new List<Popup>();
        readonly List<ISprite> images = new List<ISprite>();
        readonly List<UIText> texts = new List<UIText>();
        readonly List<IColoredRect> areas = new List<IColoredRect>();
        readonly List<Panel> panels = new List<Panel>();
        ISprite eagle = null;
        const uint EgilPortraiIndex = 5;
        const uint PyrdacorPortraitIndex = 95;

        public bool CreditsActive => credits != null;

        public CustomOutro(Game game, Layout layout, Savegame savegame)
        {
            this.game = game;
            this.layout = layout;
            renderView = layout.RenderView;
            this.savegame = savegame;

            var hero = savegame.PartyMembers[1];
            string ProcessText(string text) => text.Replace("~HERO~", hero.Name);

            void AddConversationText(TimeSpan delay, Rect rect, int textIndex, TextColor textColor = TextColor.Bright)
            {
                AddAction(delay, new ShowConversationTextAction(rect,
                    ProcessText(LanguageDependentStrings[game.GameLanguage][textIndex]), 50, textColor));
            }

            Position conversationImagePosition = new Position(31, 64);
            Rect conversationArea = new Rect(62, 61, 120, 56);

            AddAction(TimeSpan.Zero, new ShowConversationPortraitAction(EgilPortraiIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 0);
            AddConversationText(TimeSpan.FromSeconds(1), conversationArea, 1);
            AddAction(TimeSpan.FromSeconds(1), new ClearAction());
            AddAction(TimeSpan.FromMilliseconds(250), new ShowConversationPortraitAction(hero.PortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 2);
            AddAction(TimeSpan.FromSeconds(1), new ClearAction());
            AddAction(TimeSpan.Zero, new CustomAction(finished => game.RemovePartyMember(1, false, finished)));
            AddAction(TimeSpan.FromSeconds(3), new CustomAction(finished =>
            {
                int numMoves = 7;

                void MoveShipLeft()
                {
                    --game.CurrentSavegame.TransportLocations[0].Position.X;
                    game.UpdateTransportPosition(0);

                    if (--numMoves == 0)
                        finished?.Invoke();
                    else
                        game.AddTimedEvent(TimeSpan.FromMilliseconds(300), MoveShipLeft);
                }

                MoveShipLeft();
            }));
            AddAction(TimeSpan.FromMilliseconds(2500), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(500), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(500), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(500), new CustomAction(finished =>
            {
                game.EnableMusicChange(false);
                game.ToggleTransport();
                game.EnableMusicChange(true);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(500), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(250), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(250), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(250), new CustomAction(finished =>
            {
                game.Move(false, 0.0f, CursorType.ArrowRight);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromSeconds(2), new ShowConversationPortraitAction(hero.PortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 3);
            AddAction(TimeSpan.FromMilliseconds(2500), new ClearAction());
            AddAction(TimeSpan.FromMilliseconds(800), new CustomAction(finished =>
            {
                int numMoves = 10;
                int xPerMove = -8;
                int yPerMove = 6;
                int numDownMoves = 8;

                var travelInfoEagle = renderView.GameData.GetTravelGraphicInfo(TravelType.Eagle, CharacterDirection.Left);
                eagle = layout.AddMapCharacterSprite(new Rect(new Position(198, 58), new Size((int)travelInfoEagle.Width, (int)travelInfoEagle.Height)),
                    3 * 17 + (uint)TravelType.Eagle * 4 + 3, ushort.MaxValue);
                eagle.ClipArea = Game.Map2DViewArea;

                void MoveEagleDownLeft()
                {
                    eagle.X += xPerMove;
                    if (numDownMoves-- > 0)
                        eagle.Y += yPerMove;

                    if (--numMoves == 0)
                    {
                        game.PlayMusic(Song.TheUhOhSong);
                        game.AddTimedEvent((game.GetCurrentSongDuration() ?? TimeSpan.FromSeconds(12)) * 2 - TimeSpan.FromMilliseconds(10),
                            () => game.PlayMusic(Song.Ship));
                        finished?.Invoke();
                    }
                    else
                        game.AddTimedEvent(TimeSpan.FromMilliseconds(100), MoveEagleDownLeft);
                }

                MoveEagleDownLeft();
            }));
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(PyrdacorPortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 4);
            AddAction(TimeSpan.FromSeconds(1), new ClearAction());
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(hero.PortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 5);
            AddAction(TimeSpan.FromSeconds(1), new ClearAction());
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(PyrdacorPortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 6);
            AddConversationText(TimeSpan.FromMilliseconds(1250), conversationArea, 7);
            AddAction(TimeSpan.FromMilliseconds(1250), new ClearAction());
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(hero.PortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 8);
            AddAction(TimeSpan.FromSeconds(1), new ClearAction());
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(PyrdacorPortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 9);
            AddConversationText(TimeSpan.FromMilliseconds(1250), conversationArea, 10);
            AddAction(TimeSpan.FromSeconds(2), new ClearAction());
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(hero.PortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 11);
            AddAction(TimeSpan.FromSeconds(1), new ClearAction());
            AddAction(TimeSpan.FromSeconds(1), new ShowConversationPortraitAction(PyrdacorPortraitIndex, conversationImagePosition));
            AddConversationText(TimeSpan.FromMilliseconds(250), conversationArea, 12);
            float volume = game.AudioOutput.Volume;
            AddAction(TimeSpan.FromMilliseconds(3500), new CustomAction(finished =>
            {
                float factor = 0.99f;
                void ReduceVolume()
                {
                    game.AudioOutput.Volume *= factor;
                    factor -= 0.02f;
                };
                ReduceVolume();
                for (int i = 0; i < 10; ++i)
                    game.AddTimedEvent(TimeSpan.FromMilliseconds(100 + i * 100), ReduceVolume);
                finished?.Invoke();
            }));
            AddAction(TimeSpan.FromMilliseconds(1050), new CustomAction(finished =>
            {
                game.Pause();
                game.StartSequence();
                layout.AddFadeEffect(new Rect(0, 0, Global.VirtualScreenWidth, Global.VirtualScreenHeight), Color.Black, FadeEffectType.FadeIn, Game.FadeTime);
                game.AddTimedEvent(TimeSpan.FromMilliseconds(Game.FadeTime), () =>
                {
                    Clear();
                    game.PrepareOutro();
                    game.PlayMusic(Song.VoiceOfTheBagpipe);
                    game.AudioOutput.Volume = volume;
                    ShowCredits();
                });
            }));
        }

        static readonly Dictionary<GameLanguage, List<string>> LanguageDependentStrings = new Dictionary<GameLanguage, List<string>>
        {
            { GameLanguage.German, new List<string>
                {
                    "Egil: Es war ein fantastisches Abenteuer, doch nun muss auch ich Lebewohl sagen.",
                    "Egil: Die Zwerge in Gemstone brauchen Freiwillige beim Aufbau ihrer Stadt.",
                    "~HERO~: Ich hoffe wir werden uns wiedersehen. Gute Reise mein Freund!",
                    " ^~HERO~: ...",
                    " ^Unbekannter: Nicht so schnell ~HERO~!",
                    " ^~HERO~: Wer bist du?",
                    "Unbekannter: Mein Name ist ~INK17~Pyrdacor~INK31~. Ich habe nicht viel Zeit.",
                    "Unbekannter: Aber so viel sei gesagt: Das Abenteuer ist noch nicht vorbei.",
                    " ^~HERO~: Was meinst du damit?",
                    "~INK17~Ambermoon Advanced~INK31~ - eine Erweiterung der Originalversion ist geplant.",
                    " ^~INK17~Der dritte Teil der Amber-Triologie~INK31~ ist ebenfalls geplant.",
                    " ^~HERO~: Krass!",
                    "Pyrdacor: Danke, dass du ~INK22~Ambermoon~INK31~ gespielt hast! Ich hoffe du hattest Spaß."
                }
            },
            { GameLanguage.English, new List<string>
                {
                    "Egil: It was an amazing adventure but now I have to say Goodbye.",
                    "Egil: The dwarfs of Gemstone need volunteers to rebuild their capital.",
                    "~HERO~: I hope we see each other again. Have a good trip my friend!",
                    " ^~HERO~: ...",
                    " ^Stranger: Not so fast ~HERO~!",
                    " ^~HERO~: Who are you?",
                    "Stranger: My name is ~INK17~Pyrdacor~INK31~. I don't have much time.",
                    "Stranger: But I can tell you this: The adventure is not yet over.",
                    " ^~HERO~: What are you talking about?",
                    "~INK17~Ambermoon Advanced~INK31~ - an extension of the original version is planned.",
                    " ^~INK17~The third part of the Amber trilogy~INK31~ is planned as well.",
                    " ^~HERO~: Awesome!",
                    "Pyrdacor: Thank you for playing ~INK22~Ambermoon~INK31~! I hope you had fun."
                }
            },
        };

        readonly Queue<KeyValuePair<TimeSpan, IAction>> actions = new Queue<KeyValuePair<TimeSpan, IAction>>();

        void AddAction(TimeSpan time, IAction action) => actions.Enqueue(KeyValuePair.Create(time, action));

        public void Start()
        {
            // Load initial save but use current game hero portrait and name.
            var hero = savegame.PartyMembers[1];
            game.LoadInitial(hero.Name, hero.Gender == Gender.Female, hero.PortraitIndex, newSavegame =>
            {
                // Add Egil to second slot
                newSavegame.CurrentPartyMemberIndices[1] = 8;
                // Place ship at the docks
                newSavegame.TransportLocations[0].MapIndex = 156;
                newSavegame.TransportLocations[0].Position = new Position(12, 33);
                newSavegame.TransportLocations[0].TravelType = TravelType.Ship;
                // Place horse on the docks
                newSavegame.TransportLocations[1].MapIndex = 156;
                newSavegame.TransportLocations[1].Position = new Position(16, 33);
                newSavegame.TransportLocations[1].TravelType = TravelType.Horse;
                // Place the player on the docks
                newSavegame.CurrentMapIndex = 156;
                newSavegame.CurrentMapX = 13;
                newSavegame.CurrentMapY = 33;
                newSavegame.CharacterDirection = CharacterDirection.Left;
                // Set to day
                newSavegame.Hour = 13;
                newSavegame.Minute = 37;
                newSavegame.HoursWithoutSleep = 0;
                // Activate all special items
                newSavegame.SpecialItemsActive = 0xffff;
            });
            void StopGame()
            {
                game.Pause();
                game.StartSequence();
            }
            StopGame();
            game.CursorType = CursorType.None;
            game.PlayMusic(Song.Ship);
            game.AddTimedEvent(TimeSpan.FromMilliseconds(Game.FadeTime / 2), StopGame);
            game.AddTimedEvent(TimeSpan.FromSeconds(3), ProcessActions);
        }

        void ShowCredits()
        {
            game.Pause();
            game.StartSequence();
            game.CursorType = CursorType.None;
            credits = new Credits(renderView, preAction =>
            {
                credits = null;
                game.SetClickHandler(() =>
                {
                    preAction?.Invoke();
                    game.NewGame(true);
                });
                game.EndSequence();
            });
        }

        public void Update(double deltaTime)
        {
            credits?.Update(deltaTime);
        }

        void Clear(bool keepEagle = false)
        {
            foreach (var text in texts)
                text.Destroy();
            foreach (var image in images)
                image.Delete();
            foreach (var area in areas)
                area.Delete();
            foreach (var popup in popups)
                popup.Destroy();
            foreach (var panel in panels)
                panel.Destroy();

            texts.Clear();
            images.Clear();
            areas.Clear();
            popups.Clear();
            panels.Clear();

            if (!keepEagle)
            {
                eagle?.Delete();
                eagle = null;
            }
        }

        void RemoveTexts()
        {
            foreach (var text in texts)
                text.Destroy();

            texts.Clear();
        }

        void ProcessActions()
        {
            if (actions.Count != 0)
            {
                var action = actions.Dequeue();

                void RunAction() => action.Value.Run(this, ProcessActions);

                if (action.Key == TimeSpan.Zero)
                    game.ExecuteNextUpdateCycle(RunAction);
                else
                    game.AddTimedEvent(action.Key, RunAction);
            }
            else
            {
                // Note: Finished handling is done by the last action which starts the credits.
            }
        }

        void AddImage(Layer layer, uint index, Rect rect, byte displayLayer = 0,
            byte? paletteIndex = null, bool withPortraitBackground = false)
        {
            if (withPortraitBackground)
            {
                images.Add(layout.AddSprite(rect, Graphics.UICustomGraphicOffset + (uint)UICustomGraphic.PortraitBackground,
                    52, displayLayer));
            }

            images.Add(layout.AddSprite(rect, index, paletteIndex ?? game.PrimaryUIPaletteIndex,
                withPortraitBackground ? (byte)(displayLayer + 1) : displayLayer, null, null, layer));
        }

        Popup AddPopup(Position position, int columns, int rows, byte displayLayer = 0)
        {
            var popup = new Popup(game, renderView, position, columns, rows,
                false, (byte)Math.Max(0, displayLayer - Popup.BaseDisplayLayer));

            popups.Add(popup);

            return popup;
        }

        UIText AddText(string text, Rect rect, TextColor color, TextAlign textAlign = TextAlign.Left, byte displayLayer = 0)
        {
            var uiText = layout.AddText(rect, game.ProcessText(text, rect), color, textAlign, displayLayer);
            uiText.PaletteIndex = game.PrimaryUIPaletteIndex;
            texts.Add(uiText);
            return uiText;
        }

        UIText AddText(IText text, Rect rect, TextColor color, TextAlign textAlign = TextAlign.Left, byte displayLayer = 0)
        {
            var uiText = layout.AddText(rect, text, color, textAlign, displayLayer);
            uiText.PaletteIndex = game.PrimaryUIPaletteIndex;
            texts.Add(uiText);
            return uiText;
        }

        void AddArea(Rect rect, Color color, byte displayLayer = 0)
        {
            var area = renderView.ColoredRectFactory.Create(rect.Width, rect.Height, color, displayLayer);
            area.Layer = renderView.GetLayer(Layer.UI);
            area.X = rect.X;
            area.Y = rect.Y;
            area.Visible = true;
            areas.Add(area);
        }

        void AddPanel(Rect rect, byte displayLayer = 0)
        {
            panels.Add(layout.AddPanel(rect, displayLayer));
        }

        interface IAction
        {
            void Run(CustomOutro outro, Action finished);
        }

        class ShowConversationPortraitAction : IAction
        {
            readonly uint portraitIndex;
            readonly Rect rect;

            public ShowConversationPortraitAction(uint portraitIndex, Position position)
            {
                this.portraitIndex = portraitIndex;
                rect = new Rect(position, new Size(32, 32));
            }

            public void Run(CustomOutro outro, Action finished)
            {
                outro.AddPopup(rect.Position - new Position(15, 16), 11, 4);
                outro.AddImage(Layer.UI, Graphics.PortraitOffset + portraitIndex - 1, rect, Popup.BaseDisplayLayer + 1, null, true);
                finished?.Invoke();
            }
        }

        class ClearAction : IAction
        {
            public void Run(CustomOutro outro, Action finished)
            {
                outro.Clear(true);
                finished?.Invoke();
            }
        }

        class CustomAction : IAction
        {
            readonly Action<Action> action;

            public CustomAction(Action<Action> action)
            {
                this.action = action;
            }

            public void Run(CustomOutro outro, Action finished)
            {
                action?.Invoke(finished);
            }
        }

        class ShowConversationTextAction : IAction
        {
            readonly Rect rect;
            readonly string text;
            readonly int millisecondsPerCharacter;
            readonly TextColor textColor;

            public ShowConversationTextAction(Rect rect, string text, int millisecondsPerCharacter,
                TextColor textColor)
            {
                this.rect = new Rect(rect);
                this.text = text;
                this.millisecondsPerCharacter = millisecondsPerCharacter;
                this.textColor = textColor;
            }

            public void Run(CustomOutro outro, Action finished)
            {
                outro.RemoveTexts();

                int processedLines = 0;
                int processedLineCharacters = 1;
                int processedTextLength = 1;
                var textRect = rect.CreateShrinked(2);
                var clip = new Rect(textRect.Position, new Size(Global.GlyphWidth, Global.GlyphLineHeight));
                var wrappedText = outro.game.ProcessText(text, textRect);
                UIText[] texts = new UIText[wrappedText.LineCount];
                var position = new Position(textRect.Position);
                int totalLength = wrappedText.GlyphIndices.Count(c => c < (byte)SpecialGlyph.NoTrim);
                int lineSize = wrappedText.Lines[0].Count(c => c < (byte)SpecialGlyph.NoTrim);
                var currentTextColor = textColor;

                for (int i = 0; i < wrappedText.LineCount; ++i)
                {
                    var textLine = outro.renderView.TextProcessor.GetLines(wrappedText, i, 1);
                    texts[i] = outro.AddText(textLine, new Rect(position, new Size(textRect.Width, Global.GlyphLineHeight)),
                        currentTextColor, TextAlign.Left, Popup.BaseDisplayLayer + 2);
                    texts[i].Visible = i == 0;
                    texts[i].Clip(clip);
                    clip = clip.CreateModified(0, Global.GlyphLineHeight, 0, 0);
                    if (i == 0)
                        clip.Size.Width = 0;
                    position.Y += Global.GlyphLineHeight;
                    var colorChange = textLine.GlyphIndices.LastOrDefault(c => c >= (byte)SpecialGlyph.FirstColor);
                    if (colorChange != 0)
                        currentTextColor = (TextColor)(colorChange - SpecialGlyph.FirstColor);
                }
                DrawNextCharacter();

                void DrawNextCharacter()
                {
                    if (processedTextLength == totalLength)
                        finished?.Invoke();
                    else
                    {
                        ++processedTextLength;
                        texts[processedLines].IncreaseClipWidth(Global.GlyphWidth);
                        if (++processedLineCharacters == lineSize)
                        {
                            ++processedLines;
                            processedLineCharacters = 0;
                            if (processedLines < texts.Length)
                            {
                                texts[processedLines].Visible = true;
                                lineSize = wrappedText.Lines[processedLines].Count(c => c < (byte)SpecialGlyph.NoTrim);
                            }
                        }
                        outro.game.AddTimedEvent(TimeSpan.FromMilliseconds(millisecondsPerCharacter), DrawNextCharacter);
                    }
                }
            }
        }
    }
}
