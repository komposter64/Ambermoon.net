﻿/*
 * Tutorial.cs - Game introduction sequence
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

using Ambermoon.Render;
using Ambermoon.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambermoon
{
    internal class Tutorial
    {
        static readonly Dictionary<GameLanguage, string[]> texts = new Dictionary<GameLanguage, string[]>
        {
            { GameLanguage.German, new string[]
            {
                // Introduction
                "Hi ~SELF~ und willkommen zum Ambermoon Remake.^^Möchtest du eine kleine Einführung?",
                // Tip 1
                "Die Schaltflächen am unteren rechten Bildschirmrand enthalten sehr viele Funktionen " +
                "des Spiels. Wenn du dich auf dem Hauptbildschirm befindest, kannst du eine zweite " +
                "Belegung der Schaltflächen nutzen indem du auf den Bereich rechtsklickst oder die " +
                "Enter-Taste benutzt.",
                // Tip 2
                "Du kannst auch das NumPad auf der Tastatur nutzen um die Schaltflächen auszulösen." +
                "Die Anordnung der Tasten entspricht den Schaltflächen im Spiel. Mit der Taste 7 auf " +
                "dem NumPad würde so die Schaltfläche links oben (das Auge) ausgelöst.",
                // Tip 3
                "Im oberen Bereich siehst du die Spielerportraits. Du kannst die Portraits anklicken um " +
                "den aktiven Spieler auszuwählen. Per Rechtsklick gelangst du ins Inventar. " +
                "Die Tasten 1-6 selektieren ebenfalls den Charakter, und F1-F6 öffnen das jeweilige Inventar.",
                // Tip 4
                "Du kannst dich mit der Maus, den Tasten W, A, S, D oder auch den Pfeiltasten auf der Map "+
                "bewegen. In 2D kannst du per Rechtsklick auf die Map den Cursor umschalten und aus ihm " +
                "einen Aktionscursor machen, mit dem du Dinge untersuchen oder berühren oder aber mit " +
                "NPCs sprechen kannst.",
                // End
                "Ich bin nun still und wünsche dir viel Spaß beim Spielen von Ambermoon!"
            } },
            { GameLanguage.English, new string[]
            {
                // Introduction
                "Hi ~SELF~ and welcome to the Ambermoon Remake.^^Do you need a little introduction?",
                // Tip 1
                "The buttons in the lower right area of the screen provide many useful functions of " +
                "the game. If you are on the main screen you can toggle the buttons by pressing the " +
                "right mouse button while hovering the area or hitting the Return key. It will unlock " +
                "additional functions.",
                // Tip 2
                "You can also use the NumPad on your keyboard to control those buttons. The layout " +
                "is exactly as the in-game buttons. So hitting the key 7 will be equivalent to pressing " +
                "the upper left button (the eye).",
                // Tip 3
                "In the upper area you see the character portraits. You can click on them to select the " +
                "active player or right click them to open the inventories. The keyboard keys 1-6 will select " +
                "a player as well and keys F1-F6 will open the inventories.",
                // Tip 4
                "You can move on maps by using the mouse, keys W, A, S, D or the cursor keys. In 2D you can " +
                "right click on the map to change the cursor into an action cursor to interact with objects " +
                "or characters like NPCs.",
                // End
                "Now I'm quiet. Have fun playing Ambermoon!"
            } },
            { GameLanguage.French, new string[]
            {
                // Introduction
                "Bonjour ~SELF~ et bienvenue sur Ambermoon Remake.^^Avez-vous besoin d'une petite introduction ?",
                // Tip 1
                "Les boutons situés dans la partie inférieure droite de l'écran permettent d'accéder à " +
                "de nombreuses fonctions utiles du jeu. Si vous êtes sur l'écran principal, vous pouvez " +
                "faire basculer les boutons en appuyant sur le bouton droit de la souris tout en survolant " +
                "la zone ou en appuyant sur la touche Retour. Cela révèle des fonctions supplémentaires.",
                // Tip 2
                "Vous pouvez également utiliser le pavé numérique de votre clavier pour contrôler ces boutons." +
                "La disposition est exactement la même que celle des boutons du jeu. Ainsi, appuyer sur la " +
                "touche 7 équivaudra à appuyer sur le bouton supérieur gauche (l'œil).",
                // Tip 3
                "Dans la partie supérieure, vous voyez les portraits des personnages. Vous pouvez cliquer sur " +
                "eux pour sélectionner le joueur actif ou faire un clic droit pour ouvrir les inventaires." +
                "Les touches 1 à 6 du clavier permettent de sélectionner un joueur et les touches F1 à F6 " +
                "ouvrent les inventaires.",
                // Tip 4
                "Vous pouvez vous déplacer sur les cartes à l'aide de la souris, des touches W, A, S, D ou " +
                "des touches du curseur. En 2D, vous pouvez faire un clic droit sur la carte pour transformer " +
                "le curseur en curseur d'action afin d'interagir avec des objets ou des personnages comme les PNJ.",
                // End
                "Maintenant, je suis silencieux. Amusez-vous bien avec Ambermoon !"
            } },
            { GameLanguage.Polish, new string[]
            {
                // Introduction
                "Cześć ~SELF~, witaj w Ambermoon Remake.^^Potrzebujesz małego wprowadzenia?",
                // Tip 1
                "Przyciski w prawym dolnym rogu ekranu zapewniają wiele przydatnych funkcji " +
                "w grze. Na głównym ekranie można je przełączać , naciskając prawy " +
                "przycisk myszki nad ich obszarem lub wciskając klawisz Return. To pokaże " +
                "dodatkowe funkcje.",
                // Tip 2
                "Do obsługi tych przycisków możesz też użyć klawiatury numerycznej. Układ " +
                "jest dokładnie taki sam jak przycisków w grze. Tak więc klawisz 7 odpowiada " +
                "górnemu lewemu przyciskowi (oko).",
                // Tip 3
                "W górnej części znajdują się portrety postaci. Kliknięcie w jeden z nich wybiera " +
                "aktywną postać a prawy klik otwiera ekwipunek. Z klawiatury, klawisze 1-6 wybierają " +
                "aktywną postać a F1-F6 otworzą ekwipunek.",
                // Tip 4
                "Po mapie możesz się poruszać używając myszy, klawiszy W, A, S, D lub strzałek." +
                "W widoku 2D prawy klik na mapie zmienia kursor w ikonę interakcji z obiektami " +
                "lub postaciami takimi jak NPC.",
                // End
                "Teraz zamilknę. baw się dobrze grając w Ambermoon!"
            } }
        };

        readonly Game game;
        readonly IColoredRect[] markers = new IColoredRect[4];

        public Tutorial(Game game)
        {
            this.game = game;
        }

        string GetText(int index) => texts[game.GameLanguage][index];

        public void Run(IRenderView renderView)
        {
            game.StartSequence();
            game.ShowDecisionPopup(GetText(0), response =>
            {
                if (response == Data.PopupTextEvent.Response.Yes)
                {
                    ShowTips(renderView);
                }
                else
                {
                    game.EndSequence();
                }
            }, 4, 0, TextAlign.Center, false);
        }

        void ShowMarker(IRenderView renderView, Rect area)
        {
            var red = new Color(255, 0, 0);
            markers[0] = renderView.ColoredRectFactory.Create(area.Width, 1, red, 15);
            markers[1] = renderView.ColoredRectFactory.Create(1, area.Height - 2, red, 15);
            markers[2] = renderView.ColoredRectFactory.Create(1, area.Height - 2, red, 15);
            markers[3] = renderView.ColoredRectFactory.Create(area.Width, 1, red, 15);

            markers[0].X = area.Left;
            markers[0].Y = area.Top;
            markers[1].X = area.Left;
            markers[1].Y = area.Top + 1;
            markers[2].X = area.Right - 1;
            markers[2].Y = area.Top + 1;
            markers[3].X = area.Left;
            markers[3].Y = area.Bottom - 1;

            var layer = renderView.GetLayer(Layer.UI);

            foreach (var marker in markers)
            {
                marker.Layer = layer;
                marker.Visible = true;
            }
        }

        void HideMarker()
        {
            foreach (var marker in markers)
                marker?.Delete();
        }

        void ToggleButtons()
        {
            game.InputEnable = true;
            game.ToggleButtonGridPage();
            game.InputEnable = false;
        }

        void ShowTips(IRenderView renderView)
        {
            ShowTipChain(renderView, ShowTip1, ShowTip2, ShowTip3, ShowTip4, ShowTutorialEnd);
        }

        void ShowTipChain(IRenderView renderView, params Action<IRenderView, Action>[] tips)
        {
            ShowTipChain(renderView, tips as IEnumerable<Action<IRenderView, Action>>);
        }

        void ShowTipChain(IRenderView renderView, IEnumerable<Action<IRenderView, Action>> tips)
        {
            var count = tips.Count();

            if (count == 1)
                tips.First()?.Invoke(renderView, null);
            else
                tips.First()?.Invoke(renderView, () => ShowTipChain(renderView, tips.Skip(1)));
        }

        void ShowTip1(IRenderView renderView, Action next)
        {
            ShowMarker(renderView, new Rect(Global.ButtonGridX - 1, Global.ButtonGridY - 1,
                3 * Button.Width + 2, 3 * Button.Height + 2));
            game.ShowMessagePopup(GetText(1), next);
        }

        void ShowTip2(IRenderView renderView, Action next)
        {
            ToggleButtons();
            game.ShowMessagePopup(GetText(2), next);
        }

        void ShowTip3(IRenderView renderView, Action next)
        {
            ToggleButtons();
            HideMarker();
            ShowMarker(renderView, Global.PartyMemberPortraitArea);
            game.ShowMessagePopup(GetText(3), next);
        }

        void ShowTip4(IRenderView renderView, Action next)
        {
            HideMarker();
            ShowMarker(renderView, Game.Map2DViewArea);
            game.ShowMessagePopup(GetText(4), next);
        }

        void ShowTutorialEnd(IRenderView renderView, Action next)
        {
            HideMarker();
            game.ShowMessagePopup(GetText(5), next);
            next?.Invoke();
        }
    }
}
