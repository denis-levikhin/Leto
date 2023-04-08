using System.Collections.Generic;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;

using System.Numerics;

namespace leto
{
    public class Game : GameState
    {
     
        // ПЛЕЙСХОЛДЕРЫ ----------------------------------------
        private Texture2D background, person1, person2, person3;
        private Dialogsystem core;
        private Music music;
        private Font fontTtf ;
        // ----------------------------------------------------
	    private int ans = 0;
        private bool first_frame = true;
        private Choicebox answers;
        private string text = "Нажмите Enter чтобы продолжить!";
        private string tip = "Нажмите TAB для выхода в меню";
        private string sciene = "scene1";

        public Game(String sciene){
            this.sciene = sciene;
        }
        
        public override void Init()
        {
            fontTtf = LoadFontEx("resources/fonts/Yiggivoo UC.ttf", 20, Utils.alphabet, 550);
            answers = new Choicebox(0, Launcher.screenHeight-100, Launcher.screenWidth, 100);
            answers.Font = fontTtf;
            
            core = new Dialogsystem(sciene);
            next(null);
            first_frame = false;
            next(null);
        }

        public override void Render()
        {
            BeginDrawing();
            ClearBackground(RAYWHITE);

            bool ifFirstPersonIsNull = person1.Equals(default(Texture2D));
            bool ifSecondPersonIsNull = person2.Equals(default(Texture2D));

            if(!background.Equals(default(Texture2D)))DrawTexturePro(background, new Rectangle(0,0,background.width, background.height), new Rectangle(0, 0, 800, 600), new Vector2(0,0), 0, WHITE);
            if(!ifFirstPersonIsNull && !ifSecondPersonIsNull) 
            {
                DrawTexture(person1, 0, Launcher.screenHeight-person1.height, WHITE);
                DrawTexture(person2, Launcher.screenWidth-person2.width, Launcher.screenHeight-person2.height, WHITE);
            } else if (ifFirstPersonIsNull ^ ifSecondPersonIsNull) 
            {
                DrawTexture(person3 = person1.Equals(default(Texture2D)) ? person2 : person1, (800-person3.width)/2, 600-person3.height, WHITE);
            } // если выпадет ifFirstPersonIsNull = true && ifSecondPersonIsNull = true - то ничего рисовать не нужно

            //int textx = (int)(800-defaultFont.MeasureString(text).X)/2;
            //int texty = (int)(550+(50-defaultFont.MeasureString(text).Y)/2);
            DrawTextEx(fontTtf, tip, new Vector2(0, 0), 12, 1, BLACK);

            DrawRectangle(0, 500, 800, 105, ColorAlpha(DARKGRAY, 0.6f));

            //перед отображением списка ответов, нужно ещё два шага - переход к реплике, на которую нужно ответить (1 Enter) подтверждение прочтения (2 Enter)
            if(core.getState() == Dialogsystem.WAITING_ANSWER_STATE && replicaBeforeAnswersShown >= 2) answers.Render();
            else DrawTextRec(fontTtf, text, new Rectangle(20, 505, 760, 100), 18, 1, true, WHITE);
            EndDrawing();
        }

        int nextReplicaCooldown = 30; 
        int mainMenuCallCooldown = 100;
        public override void Update(FSM fsm)
        {           
            if (IsKeyDown(KEY_ENTER) && nextReplicaCooldown == 30) {
                next(fsm);
                nextReplicaCooldown = 0;
            }
            if (IsKeyDown(KEY_TAB) && mainMenuCallCooldown == 100) {
                fsm.SetCurrentState(Launcher.MAIN_MENU);
                mainMenuCallCooldown = 0;
            }
            if(nextReplicaCooldown < 30) nextReplicaCooldown++;
            if(mainMenuCallCooldown < 100) mainMenuCallCooldown++;
            if(!music.Equals(default(Music))) UpdateMusicStream(music); 
            answers.Update();
        }
  
        int replicaBeforeAnswersShown = 0;
        private void next(FSM fsm)
        {   
            if(core.getState()==Dialogsystem.WAITING_ANSWER_STATE){
                if(answers.values.Count == 0)
                foreach(var ans in core.getCurrentOptionsList()) 
                    answers.values.Add(ans.text);
                replicaBeforeAnswersShown++;
            } else replicaBeforeAnswersShown = 0; 

            if(core.getState()==Dialogsystem.WAITING_ANSWER_STATE && answers.Selected != Choicebox.NOTHING) 
            {
                core.setAnswer(answers.Selected);
                //Console.WriteLine($"my answer is {core.getCurrentOptionsList()[answers.Selected].text}");
                answers.Selected = Choicebox.NOTHING;
                answers.values.Clear();
                core.next();     
            }

            Console.WriteLine(core.getCurrentReplica().ToString());
            
            if(!core.getCurrentReplica().background.Equals(default(Texture2D))) background = core.getCurrentReplica().background;
            if(!core.getCurrentReplica().music.Equals(default(Music)) && !first_frame) {
                // UnloadMusicStream(music) здесь не воткнёшь - что если мы вернёмся к этому фрейму по цепочке, а ресурс уже выгружен?
                music = core.getCurrentReplica().music; 
                PlayMusicStream(music);
                music.looping = 1;
            }
            this.person1 = core.getCurrentReplica().person1;
            this.person2 = core.getCurrentReplica().person2;
            text = core.getCurrentReplica().text;
            PlaySound(core.getCurrentReplica().sound);

            if(core.hasNext()) {
                core.next(); 
            } else fsm.SetCurrentState(Launcher.MAIN_MENU);
        }
        public string getCurrentSciene(){
            return core.getCurrentSciene();
        }
        
        public override void Dispose()
        {
            UnloadFont(fontTtf); 
            UnloadTexture(person1);
            UnloadTexture(person2);
            UnloadTexture(background);
        }
    }
}
