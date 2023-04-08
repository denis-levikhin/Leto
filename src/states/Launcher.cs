using System.Collections.Generic;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;


namespace leto
{

    public class Launcher {

        public static readonly int screenWidth = 800;
        public static readonly int screenHeight = 600;

        public static readonly int MAIN_MENU = 1;
        public static readonly int GAME = 2;


        public static int Main()
        {
            InitWindow(screenWidth, screenHeight, "Leto");
            InitAudioDevice();
            SetTargetFPS(60); 

            FSM game = new FSM();
            var mainMenu = new MainMenu();
            game.AddState(MAIN_MENU, mainMenu);
            //game.AddState(GAME, new Game("sciene1"));
            
            game.SetCurrentState(MAIN_MENU);
            //game.SetCurrentState(GAME);
            
            while (!WindowShouldClose()) 
            {
                game.Update();
                game.Render();
            }   

            game.Clear();
            return 0;
        }

    }

}