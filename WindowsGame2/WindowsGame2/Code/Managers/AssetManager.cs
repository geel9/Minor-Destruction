using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace MiningGame.Code.Managers
{
    public static class AssetManager
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
        public static Dictionary<string, Sound> Sounds = new Dictionary<string, Sound>();
        public static Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
        public static Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        public static bool LoadAsset<T>(string key, string assetName, ContentManager content, bool absolute = false)
        {
            try
            {
                if (typeof(T) == typeof(Texture2D))
                {
                    if (GetTexture(key, false) != null) return false;
                    string path = DirectoryManager.TEXTURES + assetName;
                    if (absolute)
                        path = assetName;
                    FileStream fs = new FileStream(path, FileMode.Open);
                    Texture2D texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, fs);

                    Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
                    texture.GetData(data);

                    for (int i = 0; i < data.Length; i++)
                    {
                        Microsoft.Xna.Framework.Color c = data[i];
                        if (c.R == 255 && c.G == 0 && c.B == 174)
                        {
                            c.R = 0;
                            c.G = 0;
                            c.B = 0;
                            c.A = 0;
                            data[i] = c;
                        }
                    }

                    texture.SetData(data);

                    Textures.Add(key, texture);
                    fs.Close();
                }

                if (typeof(T) == typeof(Animation))
                {
                    if (GetAnimation(key, false) != null) return false;
                    string path = DirectoryManager.ANIMATIONS + assetName;
                    string lines = FileReaderManager.ReadFileContents(path);
                    Animation a = (Animation)JSONManager.Deserialize(lines);
                    Animations.Add(key, a);
                    ConsoleManager.Log("Added animation " + key + " with " + a.numberFrames + " frames.", Microsoft.Xna.Framework.Color.Green);
                }

                if (typeof(T) == typeof(SpriteFont))
                {
                    if (GetFont(key, false) != null) return false;
                    SpriteFont font = content.Load<SpriteFont>(assetName);
                    Fonts.Add(key, font);
                }

                if (typeof(T) == typeof(Effect))
                {
                    if (GetEffect(key, false) != null) return false;
                    Effect e = content.Load<Effect>(assetName);
                    Effects.Add(key, e);
                }

                if (typeof(T) == typeof(Sound))
                {
                    if (GetSound(key, false) != null) return false;
                    string path = DirectoryManager.SOUNDS + assetName;
                    if (absolute)
                        path = assetName;
                    SoundBuffer b = new SoundBuffer(path);
                    Sound s = new Sound(b);
                    Sounds.Add(key, s);
                    ConsoleManager.Log("Added sound " + key, Microsoft.Xna.Framework.Color.ForestGreen);
                }
            }
            catch (Exception e)
            {
                ConsoleManager.Log("Exception loading " + assetName + ": " + e.ToString());

                return false;
            }
            return true;
        }



        public static Texture2D GetTexture(string key, bool error = true)
        {
            if (Textures.ContainsKey(key))
            {
                Texture2D texture;
                Textures.TryGetValue(key, out texture);
                return texture;
            }
            else
            {
                if (error)
                    return GetTexture("error", false);
                return null;
            }
        }

        public static Animation GetAnimation(string key, bool error = true)
        {
            if (Animations.ContainsKey(key))
            {
                Animation anim;
                Animations.TryGetValue(key, out anim);
                return anim;
            }
            else
            {
                return null;
            }
        }


        public static Effect GetEffect(string key, bool error = true)
        {
            if (Effects.ContainsKey(key))
            {
                Effect e;
                Effects.TryGetValue(key, out e);
                return e;
            }
            if (error)
                return GetEffect("error");
            return null;
        }

        public static Sound GetSound(string key, bool error = true)
        {
            if (Sounds.ContainsKey(key))
            {
                Sound sound;
                Sounds.TryGetValue(key, out sound);
                return sound;
            }
            else
            {
                if (!error) return null;
                string path = DirectoryManager.SOUNDS;
                foreach (string f in Directory.GetFiles(path))
                {
                    string filename = f.Replace(path, "").Replace(".wav", "").Replace(".ogg", "");
                    if (filename == key)
                    {
                        string extension = f.Replace(path, "").Replace(key, "");
                        if (AssetManager.LoadAsset<Sound>(key, f, null, true))
                            return GetSound(key);
                    }
                }
            }
            return null;
        }

        public static SpriteFont GetFont(string key, bool error = true)
        {
            if (Fonts.ContainsKey(key))
            {
                SpriteFont font = null;
                Fonts.TryGetValue(key, out font);
                return font;
            }

            else
            {
                if (error)
                    return GetFont("default");
                return null;
            }
        }

        public static void RemoveSound(string key)
        {
            Sounds.Remove(key);
        }
    }
}
