using System;
using System.Xml;
using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace leto
{
    class Frameloader
    { 
        // Возвращаем одно значение, а не список, так как все кадры
        // связаны друг с другом ссылками, нет нужды в коллекции
        public static Framedata load(string filename)
        {            
            // почему словарь с интовыми ключами, а не массив - потому, что размер массива заранее неизвестен
            // да и ID не обязаны идти подряд, словарь гибче
            Dictionary<Int32, Framedata> scieneDictionary = new Dictionary<int, Framedata>();

            //Console.WriteLine("----------parser at work---------");
            XmlDocument xmlDoc = null;

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreComments = true;

            using (XmlReader reader = XmlReader.Create($"resources/script/{filename}.xml", readerSettings))
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                foreach(XmlNode sciene in xmlDoc.ChildNodes) {
                    foreach(XmlNode frame in sciene.ChildNodes) {
                        Framedata data = new Framedata();
                        int id = Int32.Parse(frame.Attributes["id"].Value);

                        var music = frame.Attributes["music"];
                        if (music != null) data.music = LoadMusicStream($"resources/music/{music.Value}.ogg");

                        var sound = frame.Attributes["sound"];
                        if (sound != null) data.sound = LoadSound($"resources/sound/{sound.Value}.ogg");

                        var background = frame.Attributes["background"];
                        if (background != null) {
                            Image tempimage = LoadImage($"resources/images/{background.Value}.png");
                            data.background = LoadTextureFromImage(tempimage);
                            UnloadImage(tempimage);
                        }

                        // да-да, никакого кэша ресурсов, дубликаты изображений свободно загружаются снова и снова
                        // но.. создавать систему контроля за этим просто некогда - с маленькой игрой этот движок справится
                        // и славно - большего от него не требуется

                        var person1 = frame.Attributes["person1"];
                        if (person1 != null) {
                            Image tempimage = LoadImage($"resources/images/{person1.Value}.png");
                            data.person1 = LoadTextureFromImage(tempimage);
                            UnloadImage(tempimage);
                        }

                        var person2 = frame.Attributes["person2"];
                        if (person2 != null) {
                            Image tempimage = LoadImage($"resources/images/{person2.Value}.png");
                            data.person2 = LoadTextureFromImage(tempimage);
                            UnloadImage(tempimage);
                        }

                        var frametext = frame.Attributes["text"];
                        if (frametext != null) data.text = frametext.Value;
                        
                        var nextSciene = frame.Attributes["nextSciene"];
                        if (nextSciene != null) data.nextSciene = nextSciene.Value;
                        
                        var owner = frame.Attributes["owner"];
                        if(owner != null) {
                            switch(owner.Value){
                                case "person1":
                                    data.owner = ReplicaOwner.Person1;
                                    break;
                                case "person2":
                                    data.owner = ReplicaOwner.Person2;
                                    break;
                                case "protagonist":
                                    data.owner = ReplicaOwner.Protagonist;
                                    break;
                            }
                        }

                        if(!scieneDictionary.ContainsKey(id)) scieneDictionary.Add(id, data);
                        else 
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Parser error! {id} replica is already in list! Check your script!");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        //Console.WriteLine("----------frame {0}---------", id);
                        //Console.WriteLine(data.ToString());
                    }
                }
                

                /*
                    Одновременно и запарсить реплики и слинковать трудно - реплика, которая
                    является ответом на текущую может ещё не существовать, и ссылки на неё ещё может
                    не быть. Отсюда раздельный цикл. 
                */
                string answersString;
                foreach(XmlNode sciene in xmlDoc.ChildNodes) {
                    foreach(XmlNode frame in sciene.ChildNodes) { 
                        int currentFrameId = Int32.Parse(frame.Attributes["id"].Value);

                        var answersXML = frame.Attributes["answers"];
                        if (answersXML != null) answersString = answersXML.Value;
                        else continue;
                        string[] ansIDs = answersString.Split(" ");
                        foreach (var idstr in ansIDs) {
                            int tempid = Int32.Parse(idstr);
                            scieneDictionary[currentFrameId].answers.Add(scieneDictionary[tempid]);
                        }
                    }
                }
                return scieneDictionary[0];
            }
        }
    }
}