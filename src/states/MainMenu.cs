using System.Collections.Generic;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;
using System.IO;
using System.Threading.Tasks;

using System.Numerics;

namespace leto
{
    public class MainMenu : GameState
    {
     
        Texture2D background;
        Font fontTtf ;
	    int ans = 0;
        Choicebox answers;
        string text = "Выберите пункт меню стрелками, и нажмите ENTER";
        private string tip = "Нажмите TAB чтобы вернуться в игру";
        private string log = "";
        
        public override void Init()
        {
            fontTtf = LoadFontEx("resources/fonts/Yiggivoo UC.ttf", 30, Utils.alphabet, 550);
            answers = new Choicebox((Launcher.screenWidth-200)/2, (Launcher.screenHeight-400)/2, 200, 400);
            answers.Font = fontTtf;
            answers.values.Add("Новая игра");
            answers.values.Add("Сохранить");
            answers.values.Add("Загрузить");
            answers.values.Add("Выйти");
            
            background = LoadTexture("resources/images/background.png");
        }

        public override void Render()
        {
            BeginDrawing();
            ClearBackground(RAYWHITE);
            DrawTexturePro(background, new Rectangle(0,0,background.width, background.height), new Rectangle(0, 0, 800, 600), new Vector2(0,0), 0, WHITE);
            DrawRectangle((Launcher.screenWidth-300)/2, 0, 300, Launcher.screenHeight, ColorAlpha(DARKGRAY, 0.6f));
            DrawTextEx(fontTtf, text, new Vector2((800-400)/2, Launcher.screenHeight-30), 14, 1, RAYWHITE);
            if(gameStarted) DrawTextEx(fontTtf, tip, new Vector2(0, 0), 12, 1, BLACK);
            DrawTextEx(fontTtf, log, new Vector2((800-400)/2, Launcher.screenHeight-60), 14, 1, thereIsAnError ? RED : GREEN);
            answers.Render();
            EndDrawing();
        }

        int actionCooldown = 30, logCooldown = 100;
        bool gameStarted = false;
        bool thereIsAnError = false;
        public override void Update(FSM fsm)
        {           
            answers.Update();
            if (IsKeyDown(KEY_ENTER) && actionCooldown == 30) {
                switch(answers.Selected) {
                case 0:
                    if(fsm.hasState(Launcher.GAME)) fsm.RemoveState(Launcher.GAME);
                    fsm.AddState(Launcher.GAME, new Game("scene1"));                
                    fsm.SetCurrentState(Launcher.GAME);
                    gameStarted = true;
                    break;
                case 1:
                    if(fsm.hasState(Launcher.GAME)) {
                        var game = (Game)fsm.GetAllStates()[1];
                        string sciene = game.getCurrentSciene();
                        if(sciene != null) Save(sciene);
                        thereIsAnError = false;
                        log = "Успех! НО! Внимание, сохраняется только сцена, а не текущий кадр.";
                    } else {
                        thereIsAnError = true;
                        log = "No game running.";
                    }
                    break;
                case 2:
                    if(fsm.hasState(Launcher.GAME)) fsm.RemoveState(Launcher.GAME);
                    string loadedSciene = File.ReadAllText(@"resources/savefile.save");
                    fsm.AddState(Launcher.GAME, new Game(loadedSciene));                
                    fsm.SetCurrentState(Launcher.GAME);
                    gameStarted = true;
                    break;
                    case 3: 
                      CloseWindow();
                    break;
                }
                actionCooldown = 0;
                logCooldown=0;
            }
            if (IsKeyDown(KEY_TAB) && fsm.GetAllStates().Count > 1 && actionCooldown == 30) {
                fsm.SetCurrentState(Launcher.GAME);
                actionCooldown = 0;
            }
            if(actionCooldown < 30) actionCooldown++;
            if(logCooldown < 100) logCooldown++;
            else log = "";
        }

         public void Save(string scienename) 
        {
            File.WriteAllText("resources/savefile.save", scienename);
        }
        
        public override void Leave()
        {
            actionCooldown = 0;
            logCooldown = 100;
        }
        
        public override void Dispose()
        {
            UnloadFont(fontTtf); 
            UnloadTexture(background);
        }
    }
}
