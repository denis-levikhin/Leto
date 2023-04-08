using System.Collections.Generic;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;

using System.Numerics;

namespace leto
{
    public class Choicebox 
    {
        public static readonly int NOTHING = -1;

        private int x, y, width, height;
        private Color color = Color.BLACK;
        private Color textcolor = Color.GREEN;
        public List<string> values = new List<string>();
        private Rectangle textBoundary = new Rectangle();
        private int textgap = 3;
        int selected = NOTHING;
        private Font font;

        public Choicebox(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            font = LoadFontEx("resources/fonts/Yiggivoo UC.ttf", 14, Utils.alphabet, 550);
        }
        public Choicebox(int x, int y, int width, int height, Font font)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            font = LoadFontEx("resources/fonts/Yiggivoo UC.ttf", 14, Utils.alphabet, 550);
            this.font = font;
        }

        public void Render()
        {
            //DrawRectangle(x, y, width, height, color);
            for(int i = 0; i < values.Count; i++)
            {
                DrawTextRec(font, (i == selected ? "> " : "") + values[i], new Rectangle(x, y+(font.baseSize+textgap)*i, width, font.baseSize), font.baseSize, 2, true, textcolor);
            }
        }

        public void Update()
        {
            if(IsKeyPressed(KEY_DOWN) && selected < values.Count-1) {
                selected++;
            Console.WriteLine($"selected++, new val is {selected}");
            }
            else if(IsKeyPressed(KEY_UP) && selected > 0) {
                selected--;
            
            Console.WriteLine($"selected--, new val is {selected}");
            }
        }

        public int X 
        {
            get {return x;}
            set {x = value;}
        }

        public int Y 
        {
            get {return y;}
            set {y = value;}
        }

        public int Width 
        {
            get {return width;}
            set {width = value >=0 ? value : 0;}
        }

        public int Height 
        {
            get {return height;}
            set {height = value >=0 ? value : 0;}
        }

        public Color Color 
        {
            get {return color;}
            set {color = value;}
        }

        public Font Font 
        {
            get {return font;}
            set 
            {
                UnloadFont(font);
                font = value;
            }
        }
        public int TextGap 
        {
            get {return textgap;}
            set {textgap = value;}
        }
        public Color TextColor 
        {
            get {return textcolor;}
            set {textcolor = value;}
        }

        public int Selected 
        {
            get {return selected;}
            set {selected = value > values.Count ? values.Count : value; }
        }

        ~Choicebox(){
            UnloadFont(font);
        }
    }
}